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
    }
}