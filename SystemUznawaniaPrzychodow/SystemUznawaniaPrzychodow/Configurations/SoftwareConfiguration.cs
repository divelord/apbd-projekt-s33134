using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SystemUznawaniaPrzychodow.Entities;

namespace SystemUznawaniaPrzychodow.Configurations;

public class SoftwareConfiguration : IEntityTypeConfiguration<Software>
{
    public void Configure(EntityTypeBuilder<Software> builder)
    {
        builder.ToTable("Software");

        builder.HasKey(s => s.SoftwareId);
        builder.Property(s => s.SoftwareName).IsRequired().HasMaxLength(100);
        builder.Property(s => s.Description).IsRequired().HasMaxLength(500);
        builder.Property(s => s.Version).IsRequired().HasMaxLength(10);
        builder.Property(s => s.Category).IsRequired().HasMaxLength(100);
        builder.Property(s => s.AnnualPrice).IsRequired().HasColumnType("decimal(18,2)");
    }
}