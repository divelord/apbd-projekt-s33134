namespace SystemUznawaniaPrzychodow.DTOs;

public class CreateCompanyClientDto
{
    public string CompanyName { get; set; } = string.Empty;
    public string Krs { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
}