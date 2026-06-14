using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SystemUznawaniaPrzychodow.Entities;

namespace SystemUznawaniaPrzychodow.Configurations;

public class ClientConfiguration : IEntityTypeConfiguration<Client>
{
    public void Configure(EntityTypeBuilder<Client> builder)
    {
        builder.ToTable("Clients")
            .HasDiscriminator<string>("ClientType")
            .HasValue<IndividualClient>("Individual")
            .HasValue<CompanyClient>("Company");

        builder.HasKey(c => c.ClientId);
        builder.Property(c => c.Address).IsRequired().HasMaxLength(200);
        builder.Property(c => c.Email).IsRequired().HasMaxLength(100);
        builder.Property(c => c.PhoneNumber).IsRequired().HasMaxLength(20);

        builder.HasQueryFilter(c => !(c is IndividualClient) || !((IndividualClient)c).IsDeleted);
    }
}

public class IndividualClientConfiguration : IEntityTypeConfiguration<IndividualClient>
{
    public void Configure(EntityTypeBuilder<IndividualClient> builder)
    {
        builder.Property(c => c.FirstName).IsRequired().HasMaxLength(50);
        builder.Property(c => c.LastName).IsRequired().HasMaxLength(50);
        builder.Property(c => c.Pesel).IsRequired().HasMaxLength(11);
        builder.HasIndex(c => c.Pesel).IsUnique();

        builder.HasData(
            new IndividualClient
            {
                ClientId = 1,
                FirstName = "Jan",
                LastName = "Kowalski",
                Pesel = "12345678901",
                IsDeleted = false,
                Email = "jan.kowalski@gmail.com",
                Address = "Warszawa 01-001",
                PhoneNumber = "123456789"
            },
            new IndividualClient
            {
                ClientId = 3,
                FirstName = "Anna",
                LastName = "Nowak",
                Pesel = "98765432109",
                IsDeleted = false,
                Email = "anna.nowak@eduplus.edu",
                Address = "Gdańsk 80-003",
                PhoneNumber = "555666777"
            },
            new IndividualClient
            {
                ClientId = 5,
                FirstName = "Piotr",
                LastName = "Zieliński",
                Pesel = "55544433322",
                IsDeleted = false,
                Email = "p.zielinski@wp.pl",
                Address = "Poznań 60-005",
                PhoneNumber = "888777666"
            }
        );
    }
}

public class CompanyClientConfiguration : IEntityTypeConfiguration<CompanyClient>
{
    public void Configure(EntityTypeBuilder<CompanyClient> builder)
    {
        builder.Property(c => c.CompanyName).IsRequired().HasMaxLength(200);
        builder.Property(c => c.Krs).IsRequired().HasMaxLength(10);
        builder.HasIndex(c => c.Krs).IsUnique();

        builder.HasData(
            new CompanyClient
            {
                ClientId = 2,
                CompanyName = "SoftPol Sp. z o.o.",
                Krs = "0000123456",
                Email = "kontakt@softpol.pl",
                Address = "Kraków 30-002",
                PhoneNumber = "987654321"
            },
            new CompanyClient
            {
                ClientId = 4,
                CompanyName = "MegaCorp S.A.",
                Krs = "0000987654",
                Email = "office@megacorp.com",
                Address = "Wrocław 50-004",
                PhoneNumber = "444333222"
            },
            new CompanyClient
            {
                ClientId = 6,
                CompanyName = "TechSolutions",
                Krs = "0000555444",
                Email = "info@techsol.pl",
                Address = "Łódź 90-006",
                PhoneNumber = "111222333"
            }
        );
    }
}