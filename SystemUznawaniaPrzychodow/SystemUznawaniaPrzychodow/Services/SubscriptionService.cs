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

        if (dto.RenewalPeriod < 1 || dto.RenewalPeriod > 24)
        {
            throw new BadRequestException("RenewalPeriod must be between 1 and 24 months");
        }

        var activeSubscription = await _dbContext.Subscriptions
            .AnyAsync(x => x.ClientId == dto.ClientId
                           && x.SoftwareId == dto.SoftwareId
                           && x.IsActive);

        if (activeSubscription)
        {
            throw new ConflictException("Client has active subscription for this software");
        }

        var today = DateOnly.FromDateTime(DateTime.Now);

        var activeContract = await _dbContext.Contracts
            .AnyAsync(x => x.ClientId == dto.ClientId
                           && x.SoftwareId == dto.SoftwareId
                           && x.IsSigned
                           && x.DateTo >= today);

        if (activeContract)
        {
            throw new ConflictException("Client has active contract for this software");
        }

        var highestDiscount = await _dbContext.Discounts
            .Where(x => x.SoftwareId == dto.SoftwareId
                        && x.Offer == "Subscription"
                        && x.DateFrom <= today
                        && x.DateTo >= today)
            .OrderByDescending(x => x.Percentage)
            .FirstOrDefaultAsync();

        var discountPercentage = highestDiscount?.Percentage ?? 0;

        var isLoyalClient =
            await _dbContext.Contracts.AnyAsync(x => x.ClientId == dto.ClientId && x.IsSigned)
            || await _dbContext.Subscriptions.AnyAsync(x => x.ClientId == dto.ClientId);

        if (isLoyalClient)
        {
            discountPercentage += 5;
        }

        var firstPaymentAmount = dto.RenewalAmount * (1 - discountPercentage / 100);

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
        var subscription = await _dbContext.Subscriptions
            .Include(x => x.Renewals)
            .FirstOrDefaultAsync(x => x.SubscriptionId == subscriptionId);

        if (subscription == null)
        {
            throw new NotFoundException("No Subscription found with the given id");
        }

        var client = await _dbContext.Clients.FirstOrDefaultAsync(x => x.ClientId == dto.ClientId);

        if (client == null)
        {
            throw new NotFoundException("No client found with the given id");
        }

        if (subscription.ClientId != dto.ClientId)
        {
            throw new ConflictException("Given client id does not match the client id provided in subscription");
        }

        if (!subscription.IsActive)
        {
            throw new ConflictException("Subscription is not active");
        }

        var today = DateOnly.FromDateTime(DateTime.Now);

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

        var nextPeriodStart = currentPeriodEnd;
        var nextPeriodEnd = nextPeriodStart.AddMonths(subscription.RenewalPeriod);

        var alreadyPaid = subscription.Renewals
            .Any(x => x.PeriodStart == nextPeriodStart && x.PeriodEnd == nextPeriodEnd);

        if (alreadyPaid)
        {
            throw new ConflictException("Subscription is already paid");
        }

        var renewalAmount = subscription.RenewalAmount * 0.95m;

        if (dto.Amount != renewalAmount)
        {
            throw new BadRequestException("All payments must be equal to the price of subscription");
        }

        var transaction = await _dbContext.Database.BeginTransactionAsync();

        try
        {
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

            await transaction.CommitAsync();
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}