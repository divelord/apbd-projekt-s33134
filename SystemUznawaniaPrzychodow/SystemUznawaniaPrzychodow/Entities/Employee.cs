namespace SystemUznawaniaPrzychodow.Entities;

public class Employee
{
    public int EmployeeId { get; set; }
    public string Login { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
}