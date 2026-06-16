using Microsoft.EntityFrameworkCore;
using SystemUznawaniaPrzychodow.Data;
using SystemUznawaniaPrzychodow.DTOs;
using SystemUznawaniaPrzychodow.Entities;
using SystemUznawaniaPrzychodow.Exceptions;

namespace SystemUznawaniaPrzychodow.Services;

public class ContractService : IContractService
{
    private readonly AppDbContext _dbContext;

    public ContractService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task CreateContractAsync(CreateContractDto dto)
    {
        // walidacja = czy podany klient istnieje w bazie
        var client = await _dbContext.Clients
            .FirstOrDefaultAsync(x => x.ClientId == dto.ClientId);

        if (client == null)
        {
            throw new NotFoundException($"Client with ID {dto.ClientId} not found");
        }

        // walidacja = czy podane oprogramowanie istnieje w bazie
        var software = await _dbContext.Software
            .FirstOrDefaultAsync(x => x.SoftwareId == dto.SoftwareId);

        if (software == null)
        {
            throw new NotFoundException($"Software with ID {dto.SoftwareId} not found");
        }

        // walidacja => czy data rozpoczęcia jest wcześniej niż data zakończenia
        if (dto.DateFrom > dto.DateTo)
        {
            throw new BadRequestException("DateFrom must be before DateTo");
        }

        // walidacja => przedział czasowy [3, 30] dni
        var daysDifference = Math.Abs(dto.DateTo.DayNumber - dto.DateFrom.DayNumber);

        if (daysDifference < 3 || daysDifference > 30)
        {
            throw new BadRequestException("Contract period must be between 3 and 30 days");
        }

        // walidacja => dodatkowy rok wsparcia
        if (dto.AdditionalSupportYears < 0 || dto.AdditionalSupportYears > 3)
        {
            throw new BadRequestException("Additional support can be up to 3 years");
        }

        // walidacja => czy klient ma już podpisany kontrakt na ten produkt
        var activeContract = await _dbContext.Contracts
            .AnyAsync(x => x.ClientId == dto.ClientId
                           && x.SoftwareId == dto.SoftwareId
                           && x.IsSigned
                           && x.DateTo >= dto.DateFrom);

        if (activeContract)
        {
            throw new ConflictException("Client has active contract for this software");
        }

        // walidacja => czy klient ma już aktywną subskrypcję nna ten produkt
        var activeSubscription = await _dbContext.Subscriptions
            .AnyAsync(x => x.ClientId == dto.ClientId
                           && x.SoftwareId == dto.SoftwareId
                           && x.IsActive);

        if (activeSubscription)
        {
            throw new ConflictException("Client has active subscription for this software");
        }

        var basePrice = software.AnnualPrice;

        var today = DateOnly.FromDateTime(DateTime.Now);

        // Wybranie najwyższej zniżki
        var highestDiscount = await _dbContext.Discounts
            .Where(x => x.SoftwareId == dto.SoftwareId
                        && x.Offer == "Contract"
                        && x.DateFrom <= today
                        && x.DateTo >= today)
            .OrderByDescending(x => x.Percentage)
            .FirstOrDefaultAsync();

        var discountPercentage = highestDiscount?.Percentage ?? 0;

        // Zniżka dla powracającego klienta
        var isReturningClient =
            await _dbContext.Contracts.AnyAsync(x => x.ClientId == dto.ClientId && x.IsSigned)
            || await _dbContext.Subscriptions.AnyAsync(x => x.ClientId == dto.ClientId);

        if (isReturningClient)
        {
            discountPercentage += 5;
        }

        // Opłata za dodatkowy rok wsparcia
        var additionalPrice = dto.AdditionalSupportYears * 1000;

        var finalPrice = basePrice * (1 - discountPercentage / 100m) + additionalPrice;

        var contract = new Contract
        {
            ClientId = dto.ClientId,
            SoftwareId = dto.SoftwareId,
            SoftwareVersion = software.Version,
            DateFrom = dto.DateFrom,
            DateTo = dto.DateTo,
            Deadline = dto.DateTo,
            IsSigned = false,
            Price = finalPrice,
            AdditionalSupportYears = dto.AdditionalSupportYears
        };

        await _dbContext.Contracts.AddAsync(contract);
        await _dbContext.SaveChangesAsync();
    }

    public async Task ProcessPaymentAsync(int contractId, CreatePaymentDto dto)
    {
        // walidacja => czy istnieje podany kontrakt w bazie
        var contract = await _dbContext.Contracts
            .Include(x => x.Payments)
            .FirstOrDefaultAsync(x => x.ContractId == contractId);

        if (contract == null)
        {
            throw new NotFoundException($"Contract with ID {contractId} not found");
        }

        // walidacja => czy podany klient w DTO istnieje w bazie
        var client = await _dbContext.Clients
            .FirstOrDefaultAsync(x => x.ClientId == dto.ClientId);

        if (client == null)
        {
            throw new NotFoundException($"Client with ID {dto.ClientId} not found");
        }

        // walidacja => czy podany klient w DTO to klient podany w kontrakcie
        if (contract.ClientId != dto.ClientId)
        {
            throw new BadRequestException("Given client ID does not match the client ID provided in contract");
        }

        // walidacja => czy podany kontrakt jest już podpisany
        if (contract.IsSigned)
        {
            throw new ConflictException("This contract is already paid and signed");
        }

        var today = DateOnly.FromDateTime(DateTime.Now);

        // Zwrot pieniędzy, jeśli minął deadline podpisania kontraktu 
        if (today > contract.Deadline)
        {
            var paymentsToReturn = contract.Payments.Where(p => !p.IsRefunded).ToList();

            if (paymentsToReturn.Any())
            {
                foreach (var p in paymentsToReturn)
                {
                    p.IsRefunded = true;
                }
            }

            await _dbContext.SaveChangesAsync();

            throw new ConflictException("Payment deadline has passed");
        }

        var transaction = await _dbContext.Database.BeginTransactionAsync();

        try
        {
            var paidAmount = contract.Payments
                .Where(x => !x.IsRefunded)
                .Sum(x => x.Amount);

            var totalAmount = paidAmount + dto.Amount;
            var toPay = contract.Price - paidAmount;

            // walidacja => czy podano poprawną wartość do zapłaty
            if (totalAmount > contract.Price)
            {
                throw new ConflictException($"All payments must be equal to the price specified in the contract\nTo pay: {toPay}");
            }

            var payment = new Payment
            {
                ContractId = contractId,
                Amount = dto.Amount,
                PaymentDate = today,
                IsRefunded = false
            };

            await _dbContext.Payments.AddAsync(payment);
            await _dbContext.SaveChangesAsync();

            if (totalAmount == contract.Price)
            {
                contract.IsSigned = true;

                await _dbContext.SaveChangesAsync();
            }

            await transaction.CommitAsync();
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}