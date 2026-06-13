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
        var client = await _dbContext.Clients.FirstOrDefaultAsync(x => x.ClientId == dto.ClientId);

        if (client == null)
        {
            throw new NotFoundException("No client found with the given id");
        }

        var software = await _dbContext.Software.FirstOrDefaultAsync(x => x.SoftwareId == dto.SoftwareId);

        if (software == null)
        {
            throw new NotFoundException("No software found with the given id");
        }

        var activeSubscription = await _dbContext.Subscriptions
            .AnyAsync(x => x.ClientId == dto.ClientId
                           && x.SoftwareId == dto.SoftwareId
                           && x.IsActive);

        if (activeSubscription)
        {
            throw new ConflictException("Client has active subscription for this software");
        }

        var activeContract = await _dbContext.Contracts
            .AnyAsync(x => x.ClientId == dto.ClientId
                           && x.SoftwareId == dto.SoftwareId
                           && x.IsSigned
                           && x.DateTo >= dto.DateFrom);

        if (activeContract)
        {
            throw new ConflictException("Client has active contract for this software");
        }

        var daysDifference = Math.Abs(dto.DateTo.DayNumber - dto.DateFrom.DayNumber);

        if (daysDifference < 3 || daysDifference > 30)
        {
            throw new BadRequestException("Contract period must be beetween 3 and 30 days");
        }

        if (dto.AdditionalSupportYears < 0 || dto.AdditionalSupportYears > 3)
        {
            throw new BadRequestException("Additional support can be up to 3 years");
        }

        var basePrice = software.AnnualPrice + (dto.AdditionalSupportYears * 1000);

        var today = DateOnly.FromDateTime(DateTime.Now);

        var highestDiscount = await _dbContext.Discounts
            .Where(x => x.SoftwareId == dto.SoftwareId
                        && x.Offer == "Contract"
                        && x.DateFrom <= today
                        && x.DateTo >= today)
            .OrderByDescending(x => x.Percentage)
            .FirstOrDefaultAsync();

        var discountPercentage = highestDiscount?.Percentage ?? 0;

        var isReturningClient =
            await _dbContext.Contracts.AnyAsync(x => x.ClientId == dto.ClientId && x.IsSigned)
            || await _dbContext.Subscriptions.AnyAsync(x => x.ClientId == dto.ClientId);

        if (isReturningClient)
        {
            discountPercentage += 5;
        }

        var finalPrice = basePrice * (1 - discountPercentage / 100);

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
        var contract = await _dbContext.Contracts
            .Include(x => x.Payments)
            .FirstOrDefaultAsync(x => x.ContractId == contractId);

        if (contract == null)
        {
            throw new NotFoundException("No contract found with the given id");
        }

        var client = await _dbContext.Clients.FirstOrDefaultAsync(x => x.ClientId == dto.ClientId);

        if (client == null)
        {
            throw new NotFoundException("No client found with the given id");
        }

        if (contract.ClientId != dto.ClientId)
        {
            throw new ConflictException("Given Client ID does not match the Client ID provided in contract");
        }

        if (contract.IsSigned)
        {
            throw new ConflictException("Contract is already paid and signed");
        }

        var today = DateOnly.FromDateTime(DateTime.Now);

        if (today > contract.Deadline)
        {
            foreach (var p in contract.Payments.Where(p => !p.IsRefunded))
            {
                p.IsRefunded = true;
            }

            await _dbContext.SaveChangesAsync();

            throw new ConflictException("Payment deadline has passed");
        }

        var transaction = await _dbContext.Database.BeginTransactionAsync();

        try
        {
            var totalAmount = contract.Payments
                .Where(x => !x.IsRefunded)
                .Sum(x => x.Amount);

            if (totalAmount + dto.Amount > contract.Price)
            {
                throw new ConflictException("All payments must be equal to the price specified in the contract");
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

            if (totalAmount + dto.Amount == contract.Price)
            {
                contract.IsSigned = true;
            }

            await _dbContext.SaveChangesAsync();

            await transaction.CommitAsync();
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}