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
    }
}