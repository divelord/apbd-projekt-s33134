using SystemUznawaniaPrzychodow.DTOs;

namespace SystemUznawaniaPrzychodow.Services;

public interface IClientService
{
    Task CreateIndividualClientAsync(CreateIndividualClientDto dto);
    Task CreateCompanyClientAsync(CreateCompanyClientDto dto);
    Task UpdateIndividualClientAsync(int id, UpdateIndividualClientDto dto);
    Task UpdateCompanyClientAsync(int id, UpdateCompanyClientDto dto);
    Task DeleteIndividualClientAsync(int id);
}