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
        var clientExists = await _dbContext.IndividualClients.AnyAsync(x => x.Pesel == dto.Pesel);

        if (clientExists)
        {
            throw new ConflictException("Individual client with this Pesel already exists");
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
        var clientExists = await _dbContext.CompanyClients.AnyAsync(x => x.Krs == dto.Krs);

        if (clientExists)
        {
            throw new ConflictException("Company client with this KRS already exists");
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
        var individualClient = await _dbContext.IndividualClients.FirstOrDefaultAsync(x => x.ClientId == id);

        if (individualClient == null)
        {
            throw new NotFoundException("Individual client with this ID not found");
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
        var companyClient = await _dbContext.CompanyClients.FirstOrDefaultAsync(x => x.ClientId == id);

        if (companyClient == null)
        {
            throw new NotFoundException("Company client with this ID not found");
        }

        companyClient.CompanyName = dto.CompanyName;
        companyClient.Address = dto.Address;
        companyClient.Email = dto.Email;
        companyClient.PhoneNumber = dto.PhoneNumber;

        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteIndividualClientAsync(int id)
    {
        var client = await _dbContext.Clients
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(x => x.ClientId == id);

        if (client == null)
        {
            throw new NotFoundException("Individual client with this ID not found");
        }

        if (client is CompanyClient)
        {
            throw new ConflictException("Company's data cannot be deleted");
        }

        if (client is IndividualClient individualClient)
        {
            if (individualClient.IsDeleted)
            {
                throw new ConflictException("This individual client is already deleted");
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