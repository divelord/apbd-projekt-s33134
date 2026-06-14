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

        builder.HasData(
            new Software
            {
                SoftwareId = 1,
                SoftwareName = "ERP System Pro",
                Description = "Zarządzanie przedsiębiorstwem",
                Version = "2026.1",
                Category = "Biznes",
                AnnualPrice = 4999.99m
            },
            new Software
            {
                SoftwareId = 2,
                SoftwareName = "EduLearn Platform",
                Description = "Platforma e-learningowa",
                Version = "4.2",
                Category = "Edukacja",
                AnnualPrice = 1499.00m
            },
            new Software
            {
                SoftwareId = 3,
                SoftwareName = "SecureVault CRM",
                Description = "System zarządzania relacjami",
                Version = "11.0",
                Category = "Biznes",
                AnnualPrice = 2999.50m
            },
            new Software
            {
                SoftwareId = 4,
                SoftwareName = "CloudDrive Core",
                Description = "Chmura dla firm",
                Version = "2.5",
                Category = "Narzędzia",
                AnnualPrice = 899.00m
            },
            new Software
            {
                SoftwareId = 5,
                SoftwareName = "HR Automate",
                Description = "Automatyzacja procesów HR",
                Version = "2026.2",
                Category = "HR",
                AnnualPrice = 2450.00m
            },
            new Software
            {
                SoftwareId = 6,
                SoftwareName = "FinAnalyse Premium",
                Description = "Analityka finansowa AI",
                Version = "1.0",
                Category = "Finanse",
                AnnualPrice = 7900.00m
            }
        );
    }
}