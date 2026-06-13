namespace SystemUznawaniaPrzychodow.Entities;

public class Contract
{
    public int ContractId { get; set; }
    public int ClientId { get; set; }
    public int SoftwareId { get; set; }
    public string SoftwareVersion { get; set; } = string.Empty;
    public DateOnly DateFrom { get; set; }
    public DateOnly DateTo { get; set; }
    public DateOnly Deadline { get; set; }
    public bool IsSigned { get; set; }
    public decimal Price { get; set; }
    public int AdditionalSupportYears { get; set; }

    public Client Client { get; set; } = null!;
    public Software Software { get; set; } = null!;

    public ICollection<Payment> Payments { get; set; } = [];
}