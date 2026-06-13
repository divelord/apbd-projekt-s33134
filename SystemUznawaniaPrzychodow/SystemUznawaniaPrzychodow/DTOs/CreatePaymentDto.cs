namespace SystemUznawaniaPrzychodow.DTOs;

public class CreatePaymentDto
{
    public int ClientId { get; set; }
    public decimal Amount { get; set; }
}