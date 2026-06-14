namespace SystemUznawaniaPrzychodow.DTOs;

public class CreateSubscriptionDto
{
    public int ClientId { get; set; }
    public int SoftwareId { get; set; }
    public string SubscriptionName { get; set; } = string.Empty;
    public int RenewalPeriod { get; set; }
    public decimal RenewalAmount { get; set; }
}