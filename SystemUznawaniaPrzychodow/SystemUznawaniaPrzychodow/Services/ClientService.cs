using Microsoft.EntityFrameworkCore;
using SystemUznawaniaPrzychodow.Data;
using SystemUznawaniaPrzychodow.DTOs;
using SystemUznawaniaPrzychodow.Entities;
using SystemUznawaniaPrzychodow.Exceptions;

namespace SystemUznawaniaPrzychodow.Services;

public class ClientService : IClientService
{
    private readonly AppDbContext _dbContext;

    public ClientService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task CreateIndividualClientAsync(CreateIndividualClientDto dto)
    {
        // walidacja => czy klient o podanym PESELu istnieje w bazie
        var clientExists = await _dbContext.IndividualClients
            .AnyAsync(x => x.Pesel == dto.Pesel);

        if (clientExists)
        {
            throw new ConflictException($"Client with Pesel {dto.Pesel} already exists");
        }

        var individualClient = new IndividualClient
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Pesel = dto.Pesel,
            Address = dto.Address,
            Email = dto.Email,
            PhoneNumber = dto.PhoneNumber,
        };

        await _dbContext.IndividualClients.AddAsync(individualClient);
        await _dbContext.SaveChangesAsync();
    }

    public async Task CreateCompanyClientAsync(CreateCompanyClientDto dto)
    {
        // walidacja => czy firma o podanym numerze KRS istnieje w bazie
        var clientExists = await _dbContext.CompanyClients
            .AnyAsync(x => x.Krs == dto.Krs);

        if (clientExists)
        {
            throw new ConflictException($"Company with KRS {dto.Krs} already exists");
        }

        var companyClient = new CompanyClient
        {
            CompanyName = dto.CompanyName,
            Krs = dto.Krs,
            Address = dto.Address,
            Email = dto.Email,
            PhoneNumber = dto.PhoneNumber
        };

        await _dbContext.CompanyClients.AddAsync(companyClient);
        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateIndividualClientAsync(int id, UpdateIndividualClientDto dto)
    {
        // walidacja => czy podany klient istnieje w bazie
        var individualClient = await _dbContext.IndividualClients
            .FirstOrDefaultAsync(x => x.ClientId == id);

        if (individualClient == null)
        {
            throw new NotFoundException($"Client with ID {id} not found");
        }

        individualClient.FirstName = dto.FirstName;
        individualClient.LastName = dto.LastName;
        individualClient.Address = dto.Address;
        individualClient.Email = dto.Email;
        individualClient.PhoneNumber = dto.PhoneNumber;

        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateCompanyClientAsync(int id, UpdateCompanyClientDto dto)
    {
        // walidacja => czy podany klient istnieje w bazie
        var companyClient = await _dbContext.CompanyClients
            .FirstOrDefaultAsync(x => x.ClientId == id);

        if (companyClient == null)
        {
            throw new NotFoundException($"Client with ID {id} not found");
        }

        companyClient.CompanyName = dto.CompanyName;
        companyClient.Address = dto.Address;
        companyClient.Email = dto.Email;
        companyClient.PhoneNumber = dto.PhoneNumber;

        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteIndividualClientAsync(int id)
    {
        // walidacja => czy podany klient istnieje w bazie
        var client = await _dbContext.Clients
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(x => x.ClientId == id);

        if (client == null)
        {
            throw new NotFoundException($"Client with ID {id} not found");
        }

        // walidacja => czy podano klienta jako firma
        if (client is CompanyClient)
        {
            throw new ConflictException("Company's data cannot be deleted");
        }

        // walidacja => czy podano klienta jako osoba fizyczna
        if (client is IndividualClient individualClient)
        {
            // walidacja => czy podany klient nie został już usunięty
            if (individualClient.IsDeleted)
            {
                throw new ConflictException("This client is already deleted");
            }

            individualClient.FirstName = "DELETED";
            individualClient.LastName = "DELETED";
            individualClient.Address = "DELETED";
            individualClient.Email = "DELETED";
            individualClient.PhoneNumber = "DELETED";
            individualClient.IsDeleted = true;
        }

        await _dbContext.SaveChangesAsync();
    }
}