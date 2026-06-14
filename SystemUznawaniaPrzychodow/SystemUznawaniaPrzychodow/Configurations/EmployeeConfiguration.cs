using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SystemUznawaniaPrzychodow.Entities;

namespace SystemUznawaniaPrzychodow.Configurations;

public class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
{
    public void Configure(EntityTypeBuilder<Employee> builder)
    {
        builder.ToTable("Employees");

        builder.HasKey(e => e.EmployeeId);
        builder.Property(e => e.Login).IsRequired().HasMaxLength(100);
        builder.HasIndex(e => e.Login).IsUnique();
        builder.Property(e => e.PasswordHash).IsRequired();
        builder.Property(e => e.Role).IsRequired().HasMaxLength(100);

        builder.HasData(
            new Employee
            {
                EmployeeId = 1,
                Login = "admin",
                PasswordHash = "admin123",
                Role = "Admin"
            },
            new Employee
            {
                EmployeeId = 2,
                Login = "user",
                PasswordHash = "user123",
                Role = "User"
            }
        );
    }
}