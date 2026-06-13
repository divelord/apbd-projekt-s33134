using SystemUznawaniaPrzychodow.DTOs;

namespace SystemUznawaniaPrzychodow.Services;

public interface IContractService
{
    Task CreateContractAsync(CreateContractDto dto);
    Task ProcessPaymentAsync(int contractId, CreatePaymentDto dto);
}