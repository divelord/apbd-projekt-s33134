using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SystemUznawaniaPrzychodow.Entities;

namespace SystemUznawaniaPrzychodow.Configurations;

public class DiscountConfiguration : IEntityTypeConfiguration<Discount>
{
    public void Configure(EntityTypeBuilder<Discount> builder)
    {
        builder.ToTable("Discounts");

        builder.HasKey(d => d.DiscountId);
        builder.Property(d => d.DiscountName).IsRequired().HasMaxLength(100);
        builder.Property(d => d.Offer).IsRequired().HasMaxLength(100);
        builder.Property(d => d.Percentage).IsRequired().HasColumnType("decimal(5,2)");
        builder.Property(d => d.DateFrom).IsRequired().HasColumnType("date");
        builder.Property(d => d.DateTo).IsRequired().HasColumnType("date");

        builder.HasOne(d => d.Software)
            .WithMany(p => p.Discounts)
            .HasForeignKey(d => d.SoftwareId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}