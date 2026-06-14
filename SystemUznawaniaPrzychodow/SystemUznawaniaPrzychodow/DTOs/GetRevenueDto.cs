namespace SystemUznawaniaPrzychodow.DTOs;

public class GetRevenueDto
{
    public int? SoftwareId { get; set; }
    public string Currency { get; set; } = "PLN";
}