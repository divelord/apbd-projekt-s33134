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

        builder.HasData(
            new Discount
            {
                DiscountId = 1,
                SoftwareId = 1,
                DiscountName = "Letnia Promocja ERP",
                Offer = "Subscription",
                Percentage = 10.00m,
                DateFrom = new DateOnly(2026, 05, 01),
                DateTo = new DateOnly(2026, 08, 31)
            },
            new Discount
            {
                DiscountId = 2,
                SoftwareId = 2,
                DiscountName = "Zimowa Promocja Edu",
                Offer = "Contract",
                Percentage = 15.00m,
                DateFrom = new DateOnly(2025, 12, 01),
                DateTo = new DateOnly(2026, 02, 28)
            },
            new Discount
            {
                DiscountId = 3,
                SoftwareId = 3,
                DiscountName = "Wiosenny CRM",
                Offer = "Contract",
                Percentage = 5.00m,
                DateFrom = new DateOnly(2026, 03, 01),
                DateTo = new DateOnly(2026, 06, 30)
            },
            new Discount
            {
                DiscountId = 4,
                SoftwareId = 4,
                DiscountName = "Cloud Start Bonus",
                Offer = "Subscription",
                Percentage = 20.00m,
                DateFrom = new DateOnly(2026, 01, 01),
                DateTo = new DateOnly(2026, 12, 31)
            },
            new Discount
            {
                DiscountId = 5,
                SoftwareId = 5,
                DiscountName = "HR New Release",
                Offer = "Contract",
                Percentage = 12.00m,
                DateFrom = new DateOnly(2026, 06, 01),
                DateTo = new DateOnly(2026, 07, 15)
            },
            new Discount
            {
                DiscountId = 6,
                SoftwareId = 6,
                DiscountName = "AI Launch Promo",
                Offer = "Subscription",
                Percentage = 25.00m,
                DateFrom = new DateOnly(2026, 06, 10),
                DateTo = new DateOnly(2026, 06, 20)
            }
        );
    }
}