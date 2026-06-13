namespace SystemUznawaniaPrzychodow.Entities;

public abstract class Client
{
    public int ClientId { get; set; }
    public string Address { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;

    public ICollection<Contract> Contracts { get; set; } = [];
    public ICollection<Subscription> Subscriptions { get; set; } = [];
}

public class IndividualClient : Client
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Pesel { get; set; } = string.Empty;
    public bool IsDeleted { get; set; } = false;
}

public class CompanyClient : Client
{
    public string CompanyName { get; set; } = string.Empty;
    public string Krs { get; set; } = string.Empty;
}