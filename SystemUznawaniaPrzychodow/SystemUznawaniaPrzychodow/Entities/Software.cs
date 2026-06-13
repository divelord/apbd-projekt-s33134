namespace SystemUznawaniaPrzychodow.Entities;

public class Software
{
    public int SoftwareId { get; set; }
    public string SoftwareName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public decimal AnnualPrice { get; set; }

    public ICollection<Contract> Contracts { get; set; } = [];
    public ICollection<Discount> Discounts { get; set; } = [];
    public ICollection<Subscription> Subscriptions { get; set; } = [];
}