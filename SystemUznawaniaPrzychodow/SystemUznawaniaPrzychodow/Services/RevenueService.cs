using Microsoft.EntityFrameworkCore;
using SystemUznawaniaPrzychodow.Data;
using SystemUznawaniaPrzychodow.DTOs;
using SystemUznawaniaPrzychodow.Exceptions;

namespace SystemUznawaniaPrzychodow.Services;

public class RevenueService : IRevenueService
{
    private readonly AppDbContext _dbContext;
    private readonly CurrencyService _currencyService;

    public RevenueService(AppDbContext dbContext, CurrencyService currencyService)
    {
        _dbContext = dbContext;
        _currencyService = currencyService;
    }

    public async Task<GetRevenueResponseDto> GetCurrentRevenueAsync(int? softwareId, string currency)
    {
        // walidacja => czy istnieje podana subskrypcja w bazie
        if (softwareId.HasValue)
        {
            var software = await _dbContext.Software
                .FirstOrDefaultAsync(x => x.SoftwareId == softwareId.Value);

            if (software == null)
            {
                throw new NotFoundException($"Software with ID {softwareId.Value} not found");
            }
        }

        var totalRevenue = await GetCurrentRevenueInPlnAsync(softwareId);
        var exchangeRate = await _currencyService.GetExchangeRateAsync(currency);

        decimal rateValue = exchangeRate ?? 1.0m;

        var revenue = new GetRevenueResponseDto
        {
            Revenue = Math.Round(totalRevenue / rateValue, 2),
            Currency = currency.ToUpper(),
        };

        return revenue;
    }

    public async Task<GetRevenueResponseDto> GetExpectedRevenueAsync(int? softwareId, string currency)
    {
        // walidacja => czy istnieje podana subskrypcja w bazie
        if (softwareId.HasValue)
        {
            var software = await _dbContext.Software
                .FirstOrDefaultAsync(x => x.SoftwareId == softwareId.Value);

            if (software == null)
            {
                throw new NotFoundException($"Software with ID {softwareId.Value} not found");
            }
        }

        var totalRevenue = await GetCurrentRevenueInPlnAsync(softwareId);

        var today = DateOnly.FromDateTime(DateTime.Now);

        var contracts = _dbContext.Contracts
            .Where(x => !x.IsSigned && x.Deadline >= today);
        var subscriptions = _dbContext.Subscriptions
            .Include(x => x.Renewals)
            .Where(x => x.IsActive);

        if (softwareId.HasValue)
        {
            contracts = contracts.Where(x => x.SoftwareId == softwareId.Value);
            subscriptions = subscriptions.Where(x => x.SoftwareId == softwareId.Value);
        }

        totalRevenue += await contracts.SumAsync(x => x.Price);

        var activeSubscriptions = await subscriptions.ToListAsync();

        foreach (var subscription in activeSubscriptions)
        {
            var lastRenewal = subscription.Renewals
                .OrderByDescending(x => x.PeriodEnd)
                .FirstOrDefault();

            var currentPeriodEnd = lastRenewal?.PeriodEnd ?? subscription.StartDate;

            if (currentPeriodEnd >= today)
            {
                totalRevenue += subscription.RenewalAmount;
            }
        }

        var exchangeRate = await _currencyService.GetExchangeRateAsync(currency);
        var rateValue = exchangeRate ?? 1.0m;

        var revenue = new GetRevenueResponseDto
        {
            Revenue = Math.Round(totalRevenue / rateValue, 2),
            Currency = currency.ToUpper(),
        };

        return revenue;
    }

    // metoda do wyliczenia aktualnego przychodu
    private async Task<decimal> GetCurrentRevenueInPlnAsync(int? softwareId)
    {
        var contracts = _dbContext.Contracts
            .Where(x => x.IsSigned);
        var subscriptions = _dbContext.SubscriptionRenewals
            .Include(x => x.Subscription)
            .Where(x => x.Subscription.IsActive);

        if (softwareId.HasValue)
        {
            contracts = contracts.Where(x => x.SoftwareId == softwareId.Value);
            subscriptions = subscriptions.Where(x => x.Subscription.SoftwareId == softwareId.Value);
        }

        var contractRevenue = await contracts.SumAsync(x => x.Price);
        var subscriptionRevenue = await subscriptions.SumAsync(x => x.AmountPaid);

        return contractRevenue + subscriptionRevenue;
    }
}