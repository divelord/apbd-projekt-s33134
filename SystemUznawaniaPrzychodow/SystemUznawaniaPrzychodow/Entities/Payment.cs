namespace SystemUznawaniaPrzychodow.Entities;

public class Payment
{
    public int PaymentId { get; set; }
    public int ContractId { get; set; }
    public decimal Amount { get; set; }
    public DateOnly PaymentDate { get; set; }
    public bool IsRefunded { get; set; }

    public Contract Contract { get; set; } = null!;
}