namespace SystemUznawaniaPrzychodow.Entities;

public class Discount
{
    public int DiscountId { get; set; }
    public int SoftwareId { get; set; }
    public string DiscountName { get; set; } = string.Empty;
    public string Offer { get; set; } = string.Empty;
    public decimal Percentage { get; set; }
    public DateOnly DateFrom { get; set; }
    public DateOnly DateTo { get; set; }

    public Software Software { get; set; } = null!;
}