using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SystemUznawaniaPrzychodow.Entities;

namespace SystemUznawaniaPrzychodow.Configurations;

public class SubscriptionRenewalConfiguration : IEntityTypeConfiguration<SubscriptionRenewal>
{
    public void Configure(EntityTypeBuilder<SubscriptionRenewal> builder)
    {
        builder.ToTable("SubscriptionRenewals");

        builder.HasKey(s => s.RenewalId);
        builder.Property(s => s.AmountPaid).IsRequired().HasColumnType("decimal(18,2)");
        builder.Property(s => s.PaymentDate).IsRequired().HasColumnType("date");
        builder.Property(s => s.PeriodStart).IsRequired().HasColumnType("date");
        builder.Property(s => s.PeriodEnd).IsRequired().HasColumnType("date");

        builder.HasOne(s => s.Subscription)
            .WithMany(s => s.Renewals)
            .HasForeignKey(s => s.SubscriptionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasData(
            new SubscriptionRenewal
            {
                RenewalId = 1,
                SubscriptionId = 1,
                AmountPaid = 190.00m,
                PaymentDate = new DateOnly(2026, 04, 15),
                PeriodStart = new DateOnly(2026, 04, 15),
                PeriodEnd = new DateOnly(2026, 05, 15)
            },
            new SubscriptionRenewal
            {
                RenewalId = 2,
                SubscriptionId = 1,
                AmountPaid = 190.00m,
                PaymentDate = new DateOnly(2026, 05, 14),
                PeriodStart = new DateOnly(2026, 05, 15),
                PeriodEnd = new DateOnly(2026, 06, 15)
            },
            new SubscriptionRenewal
            {
                RenewalId = 3,
                SubscriptionId = 2,
                AmountPaid = 500.00m,
                PaymentDate = new DateOnly(2026, 05, 01),
                PeriodStart = new DateOnly(2026, 05, 01),
                PeriodEnd = new DateOnly(2026, 06, 01)
            },
            new SubscriptionRenewal
            {
                RenewalId = 4,
                SubscriptionId = 2,
                AmountPaid = 500.00m,
                PaymentDate = new DateOnly(2026, 05, 30),
                PeriodStart = new DateOnly(2026, 06, 01),
                PeriodEnd = new DateOnly(2026, 07, 01)
            },
            new SubscriptionRenewal
            {
                RenewalId = 5,
                SubscriptionId = 3,
                AmountPaid = 1125.00m,
                PaymentDate = new DateOnly(2026, 06, 11),
                PeriodStart = new DateOnly(2026, 06, 11),
                PeriodEnd = new DateOnly(2026, 09, 11)
            },
            new SubscriptionRenewal
            {
                RenewalId = 6,
                SubscriptionId = 4,
                AmountPaid = 300.00m,
                PaymentDate = new DateOnly(2026, 05, 10),
                PeriodStart = new DateOnly(2026, 05, 10),
                PeriodEnd = new DateOnly(2026, 06, 10)
            }
        );
    }
}