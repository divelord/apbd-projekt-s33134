using SystemUznawaniaPrzychodow.DTOs;

namespace SystemUznawaniaPrzychodow.Services;

public interface ISubscriptionService
{
    Task CreateSubscriptionAsync(CreateSubscriptionDto dto);
    Task ProcessRenewalAsync(int subscriptionId, CreateSubscriptionRenewalDto dto);
}