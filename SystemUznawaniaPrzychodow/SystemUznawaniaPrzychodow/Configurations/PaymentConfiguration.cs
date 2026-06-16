using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SystemUznawaniaPrzychodow.Entities;

namespace SystemUznawaniaPrzychodow.Configurations;

public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.ToTable("Payments");

        builder.HasKey(p => p.PaymentId);
        builder.Property(p => p.Amount).IsRequired().HasColumnType("decimal(18,2)");
        builder.Property(p => p.PaymentDate).IsRequired().HasColumnType("date");
        builder.Property(p => p.IsRefunded).IsRequired().HasColumnType("bit");

        builder.HasOne(p => p.Contract)
            .WithMany(c => c.Payments)
            .HasForeignKey(p => p.ContractId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasData(
            new Payment
            {
                PaymentId = 1,
                ContractId = 1,
                Amount = 5000.00m,
                PaymentDate = new DateOnly(2026, 01, 20),
                IsRefunded = false
            },
            new Payment
            {
                PaymentId = 2,
                ContractId = 2,
                Amount = 4000.00m,
                PaymentDate = new DateOnly(2026, 02, 15),
                IsRefunded = false
            },
            new Payment
            {
                PaymentId = 3,
                ContractId = 2,
                Amount = 4000.00m,
                PaymentDate = new DateOnly(2026, 02, 20),
                IsRefunded = false
            },
            new Payment
            {
                PaymentId = 4,
                ContractId = 3,
                Amount = 3000.00m,
                PaymentDate = new DateOnly(2026, 05, 05),
                IsRefunded = false
            },
            new Payment
            {
                PaymentId = 5,
                ContractId = 1,
                Amount = 5000.00m,
                PaymentDate = new DateOnly(2026, 01, 21),
                IsRefunded = true
            },
            new Payment
            {
                PaymentId = 6,
                ContractId = 3,
                Amount = 1500.00m,
                PaymentDate = new DateOnly(2026, 05, 06),
                IsRefunded = true
            }
        );
    }
}