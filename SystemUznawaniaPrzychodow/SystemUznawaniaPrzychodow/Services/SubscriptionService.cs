using Microsoft.EntityFrameworkCore;
using SystemUznawaniaPrzychodow.Data;
using SystemUznawaniaPrzychodow.DTOs;
using SystemUznawaniaPrzychodow.Entities;
using SystemUznawaniaPrzychodow.Exceptions;

namespace SystemUznawaniaPrzychodow.Services;

public class SubscriptionService : ISubscriptionService
{
    private readonly AppDbContext _dbContext;

    public SubscriptionService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task CreateSubscriptionAsync(CreateSubscriptionDto dto)
    {
        // walidacja => czy podany klient istnieje w bazie
        var client = await _dbContext.Clients
            .FirstOrDefaultAsync(x => x.ClientId == dto.ClientId);

        if (client == null)
        {
            throw new NotFoundException($"Client with ID {dto.ClientId} not found");
        }

        // walidacja => czy podane oprogramowanie istnieje w bazie
        var software = await _dbContext.Software
            .FirstOrDefaultAsync(x => x.SoftwareId == dto.SoftwareId);

        if (software == null)
        {
            throw new NotFoundException($"Software with ID {dto.SoftwareId} not found");
        }

        // walidacja => czas odnowienia w przedziale [1, 24] miesięcy
        if (dto.RenewalPeriod < 1 || dto.RenewalPeriod > 24)
        {
            throw new BadRequestException("RenewalPeriod must be between 1 and 24 months");
        }

        var today = DateOnly.FromDateTime(DateTime.Now);

        // walidacja => czy klient ma już podpisany kontrakt na ten produkt
        var activeContract = await _dbContext.Contracts
            .AnyAsync(x => x.ClientId == dto.ClientId
                           && x.SoftwareId == dto.SoftwareId
                           && x.IsSigned
                           && x.DateTo >= today);

        if (activeContract)
        {
            throw new ConflictException("Client has active contract for this software");
        }

        // walidacja => czy klient ma już aktywną subskrypcję na ten produkt
        var activeSubscription = await _dbContext.Subscriptions
            .AnyAsync(x => x.ClientId == dto.ClientId
                           && x.SoftwareId == dto.SoftwareId
                           && x.IsActive);

        if (activeSubscription)
        {
            throw new ConflictException("Client has active subscription for this software");
        }

        // Wybranie najwyższej zniżki
        var highestDiscount = await _dbContext.Discounts
            .Where(x => x.SoftwareId == dto.SoftwareId
                        && x.Offer == "Subscription"
                        && x.DateFrom <= today
                        && x.DateTo >= today)
            .OrderByDescending(x => x.Percentage)
            .FirstOrDefaultAsync();

        var discountPercentage = highestDiscount?.Percentage ?? 0;

        // Zniżka dla lojalnego klienta
        var isLoyalClient =
            await _dbContext.Contracts.AnyAsync(x => x.ClientId == dto.ClientId && x.IsSigned)
            || await _dbContext.Subscriptions.AnyAsync(x => x.ClientId == dto.ClientId);

        if (isLoyalClient)
        {
            discountPercentage += 5;
        }

        var firstPaymentAmount = dto.RenewalAmount * (1 - discountPercentage / 100m);

        var periodStart = today;
        var periodEnd = today.AddMonths(dto.RenewalPeriod);

        var transaction = await _dbContext.Database.BeginTransactionAsync();

        try
        {
            var subscription = new Subscription
            {
                ClientId = dto.ClientId,
                SoftwareId = dto.SoftwareId,
                SubscriptionName = dto.SubscriptionName,
                RenewalPeriod = dto.RenewalPeriod,
                RenewalAmount = dto.RenewalAmount,
                IsActive = true,
                StartDate = today,
            };

            await _dbContext.Subscriptions.AddAsync(subscription);
            await _dbContext.SaveChangesAsync();

            var renewal = new SubscriptionRenewal
            {
                SubscriptionId = subscription.SubscriptionId,
                AmountPaid = firstPaymentAmount,
                PaymentDate = today,
                PeriodStart = periodStart,
                PeriodEnd = periodEnd,
            };

            await _dbContext.SubscriptionRenewals.AddAsync(renewal);
            await _dbContext.SaveChangesAsync();

            await transaction.CommitAsync();
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task ProcessRenewalAsync(int subscriptionId, CreateSubscriptionRenewalDto dto)
    {
        // walidacja => czy istnieje podana subskrypcja w bazie
        var subscription = await _dbContext.Subscriptions
            .Include(x => x.Renewals)
            .FirstOrDefaultAsync(x => x.SubscriptionId == subscriptionId);

        if (subscription == null)
        {
            throw new NotFoundException("No Subscription found with the given id");
        }

        // walidacja => czy podany klient w DTO istnieje w bazie
        var client = await _dbContext.Clients
            .FirstOrDefaultAsync(x => x.ClientId == dto.ClientId);

        if (client == null)
        {
            throw new NotFoundException($"Client with ID {dto.ClientId} not found");
        }

        // walidacja => czy podany klient w DTO to klient związany z subskrypcją
        if (subscription.ClientId != dto.ClientId)
        {
            throw new BadRequestException("Given client ID does not match the client ID provided in subscription");
        }

        var today = DateOnly.FromDateTime(DateTime.Now);

        // walidacja => czy subskrypcja nie wygasła z powodu braków płatności 
        var lastRenewal = subscription.Renewals
            .OrderByDescending(x => x.PeriodEnd)
            .FirstOrDefault();

        var currentPeriodEnd = lastRenewal!.PeriodEnd;

        if (today > currentPeriodEnd)
        {
            subscription.IsActive = false;

            await _dbContext.SaveChangesAsync();

            throw new ConflictException("Renewal has been cancelled due to missed renewal deadline");
        }

        // walidacja => czy podana subskrypcja jest nie aktywna
        if (!subscription.IsActive)
        {
            throw new ConflictException("Subscription is not active");
        }

        // ramy czasowe do kolejnego okresu rozliczeniowego
        var nextPeriodStart = currentPeriodEnd;
        var nextPeriodEnd = nextPeriodStart.AddMonths(subscription.RenewalPeriod);

        // walidacja => czy kolejny okres rozliczeniowy nie został już opłacony
        var alreadyPaid = subscription.Renewals
            .Any(x => x.PeriodStart == nextPeriodStart && x.PeriodEnd == nextPeriodEnd);

        if (alreadyPaid)
        {
            throw new ConflictException("Subscription is already paid");
        }

        // Sprawdzenie, czy kolejne płatności mają obsługiwać zniżkę lojalnościowego klienta
        var isLoyalClient =
            await _dbContext.Contracts.AnyAsync(x => x.ClientId == dto.ClientId && x.IsSigned)
            || await _dbContext.Subscriptions.AnyAsync(x => x.ClientId == dto.ClientId
                                                            && x.SubscriptionId != subscription.SubscriptionId);

        var renewalAmount = subscription.RenewalAmount;

        if (isLoyalClient)
        {
            renewalAmount *= 0.95m;
        }

        // walidacja => czy podano poprawną wartość do zapłaty
        if (dto.Amount != renewalAmount)
        {
            throw new BadRequestException($"All payments must be equal to the price of subscription\nTo pay: {renewalAmount}");
        }

        var renewal = new SubscriptionRenewal
        {
            SubscriptionId = subscription.SubscriptionId,
            AmountPaid = dto.Amount,
            PaymentDate = today,
            PeriodStart = nextPeriodStart,
            PeriodEnd = nextPeriodEnd,
        };

        await _dbContext.SubscriptionRenewals.AddAsync(renewal);
        await _dbContext.SaveChangesAsync();
    }
}