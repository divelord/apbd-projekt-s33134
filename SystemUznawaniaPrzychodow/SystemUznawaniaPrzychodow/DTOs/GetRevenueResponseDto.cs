namespace SystemUznawaniaPrzychodow.DTOs;

public class GetRevenueResponseDto
{
    public decimal Revenue { get; set; }
    public string Currency { get; set; } = "PLN";
}