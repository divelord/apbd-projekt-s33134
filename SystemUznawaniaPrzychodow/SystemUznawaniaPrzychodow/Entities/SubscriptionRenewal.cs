namespace SystemUznawaniaPrzychodow.Entities;

public class SubscriptionRenewal
{
    public int RenewalId { get; set; }
    public int SubscriptionId { get; set; }
    public decimal AmountPaid { get; set; }
    public DateOnly PaymentDate { get; set; }
    public DateOnly PeriodStart { get; set; }
    public DateOnly PeriodEnd { get; set; }

    public Subscription Subscription { get; set; } = null!;
}