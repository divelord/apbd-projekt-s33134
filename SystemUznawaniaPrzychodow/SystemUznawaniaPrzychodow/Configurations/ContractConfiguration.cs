using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SystemUznawaniaPrzychodow.Entities;

namespace SystemUznawaniaPrzychodow.Configurations;

public class ContractConfiguration : IEntityTypeConfiguration<Contract>
{
    public void Configure(EntityTypeBuilder<Contract> builder)
    {
        builder.ToTable("Contracts");

        builder.HasKey(c => c.ContractId);
        builder.Property(c => c.SoftwareVersion).IsRequired().HasMaxLength(20);
        builder.Property(c => c.DateFrom).IsRequired().HasColumnType("date");
        builder.Property(c => c.DateTo).IsRequired().HasColumnType("date");
        builder.Property(c => c.Deadline).IsRequired().HasColumnType("date");
        builder.Property(c => c.IsSigned).IsRequired().HasColumnType("bit");
        builder.Property(c => c.Price).IsRequired().HasColumnType("decimal(18,2)");
        builder.Property(c => c.AdditionalSupportYears).IsRequired();

        builder.HasOne(c => c.Client)
            .WithMany(c => c.Contracts)
            .HasForeignKey(c => c.ClientId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(c => c.Software)
            .WithMany(s => s.Contracts)
            .HasForeignKey(c => c.SoftwareId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasData(
            new Contract
            {
                ContractId = 1,
                ClientId = 1,
                SoftwareId = 1,
                SoftwareVersion = "2026.1",
                Price = 5000.00m,
                IsSigned = true,
                DateFrom = new DateOnly(2026, 01, 15),
                DateTo = new DateOnly(2027, 01, 15),
                Deadline = new DateOnly(2026, 01, 30),
                AdditionalSupportYears = 1
            },
            new Contract
            {
                ContractId = 2,
                ClientId = 4,
                SoftwareId = 3,
                SoftwareVersion = "11.0",
                Price = 8000.00m,
                IsSigned = true,
                DateFrom = new DateOnly(2026, 02, 10),
                DateTo = new DateOnly(2027, 02, 10),
                Deadline = new DateOnly(2026, 02, 25),
                AdditionalSupportYears = 0
            },
            new Contract
            {
                ContractId = 3,
                ClientId = 5,
                SoftwareId = 5,
                SoftwareVersion = "2026.2",
                Price = 3000.00m,
                IsSigned = true,
                DateFrom = new DateOnly(2026, 05, 01),
                DateTo = new DateOnly(2027, 05, 01),
                Deadline = new DateOnly(2026, 05, 15),
                AdditionalSupportYears = 2
            },
            new Contract
            {
                ContractId = 4,
                ClientId = 2,
                SoftwareId = 1,
                SoftwareVersion = "2026.1",
                Price = 12000.00m,
                IsSigned = false,
                DateFrom = new DateOnly(2026, 07, 01),
                DateTo = new DateOnly(2027, 07, 01),
                Deadline = new DateOnly(2026, 07, 25),
                AdditionalSupportYears = 1
            },
            new Contract
            {
                ContractId = 5,
                ClientId = 6,
                SoftwareId = 4,
                SoftwareVersion = "2.5",
                Price = 4500.00m,
                IsSigned = false,
                DateFrom = new DateOnly(2026, 06, 20),
                DateTo = new DateOnly(2027, 06, 20),
                Deadline = new DateOnly(2026, 07, 08),
                AdditionalSupportYears = 0
            },
            new Contract
            {
                ContractId = 6,
                ClientId = 3,
                SoftwareId = 2,
                SoftwareVersion = "4.2",
                Price = 3500.00m,
                IsSigned = false,
                DateFrom = new DateOnly(2026, 04, 01),
                DateTo = new DateOnly(2027, 04, 01),
                Deadline = new DateOnly(2026, 05, 01),
                AdditionalSupportYears = 3
            }
        );
    }
}