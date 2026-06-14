using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SystemUznawaniaPrzychodow.Entities;

namespace SystemUznawaniaPrzychodow.Configurations;

public class SubscriptionConfiguration : IEntityTypeConfiguration<Subscription>
{
    public void Configure(EntityTypeBuilder<Subscription> builder)
    {
        builder.ToTable("Subscriptions");

        builder.HasKey(s => s.SubscriptionId);
        builder.Property(s => s.SubscriptionName).IsRequired().HasMaxLength(100);
        builder.Property(s => s.RenewalPeriod).IsRequired();
        builder.Property(s => s.RenewalAmount).IsRequired().HasColumnType("decimal(18,2)");
        builder.Property(s => s.IsActive).IsRequired().HasColumnType("bit");
        builder.Property(s => s.StartDate).IsRequired().HasColumnType("date");

        builder.HasOne(s => s.Client)
            .WithMany(c => c.Subscriptions)
            .HasForeignKey(s => s.ClientId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(s => s.Software)
            .WithMany(s => s.Subscriptions)
            .HasForeignKey(s => s.SoftwareId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasData(
            new Subscription
            {
                SubscriptionId = 1,
                ClientId = 1,
                SoftwareId = 2,
                SubscriptionName = "EduLearn Premium",
                RenewalPeriod = 1,
                RenewalAmount = 200.00m,
                IsActive = true,
                StartDate = new DateOnly(2026, 04, 15)
            },
            new Subscription
            {
                SubscriptionId = 2,
                ClientId = 3,
                SoftwareId = 4,
                SubscriptionName = "CloudDrive Business",
                RenewalPeriod = 1,
                RenewalAmount = 500.00m,
                IsActive = true,
                StartDate = new DateOnly(2026, 05, 01)
            },
            new Subscription
            {
                SubscriptionId = 3,
                ClientId = 4,
                SoftwareId = 6,
                SubscriptionName = "FinAnalyse Enterprise",
                RenewalPeriod = 3,
                RenewalAmount = 1500.00m,
                IsActive = true,
                StartDate = new DateOnly(2026, 06, 11)
            },
            new Subscription
            {
                SubscriptionId = 4,
                ClientId = 5,
                SoftwareId = 1,
                SubscriptionName = "ERP Lite Client",
                RenewalPeriod = 1,
                RenewalAmount = 300.00m,
                IsActive = true,
                StartDate = new DateOnly(2026, 03, 10)
            },
            new Subscription
            {
                SubscriptionId = 5,
                ClientId = 2,
                SoftwareId = 2,
                SubscriptionName = "EduLearn Basic",
                RenewalPeriod = 3,
                RenewalAmount = 450.00m,
                IsActive = false,
                StartDate = new DateOnly(2025, 09, 01)
            },
            new Subscription
            {
                SubscriptionId = 6,
                ClientId = 6,
                SoftwareId = 3,
                SubscriptionName = "SecureVault Promo",
                RenewalPeriod = 1,
                RenewalAmount = 100.00m,
                IsActive = false,
                StartDate = new DateOnly(2026, 01, 01)
            }
        );
    }
}