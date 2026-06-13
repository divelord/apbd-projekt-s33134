namespace SystemUznawaniaPrzychodow.DTOs;

public class CreateContractDto
{
    public int ClientId { get; set; }
    public int SoftwareId { get; set; }
    public DateOnly DateFrom { get; set; }
    public DateOnly DateTo { get; set; }
    public int AdditionalSupportYears { get; set; }
}