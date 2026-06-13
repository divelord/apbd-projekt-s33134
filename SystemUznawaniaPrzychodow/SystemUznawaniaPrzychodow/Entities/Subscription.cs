namespace SystemUznawaniaPrzychodow.Entities;

public class Subscription
{
    public int SubscriptionId { get; set; }
    public int ClientId { get; set; }
    public int SoftwareId { get; set; }
    public int RenewalPeriod { get; set; }
    public decimal RenewalAmount { get; set; }
    public bool IsActive { get; set; }
    public DateOnly StartDate { get; set; }

    public Client Client { get; set; } = null!;
    public Software Software { get; set; } = null!;

    public ICollection<SubscriptionRenewal> Renewals { get; set; } = [];
}