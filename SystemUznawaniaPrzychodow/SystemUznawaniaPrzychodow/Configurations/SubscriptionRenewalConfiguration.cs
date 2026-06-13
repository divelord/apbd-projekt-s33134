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
    }
}