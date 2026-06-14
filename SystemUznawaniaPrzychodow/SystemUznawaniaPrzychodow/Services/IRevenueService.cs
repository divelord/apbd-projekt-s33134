using SystemUznawaniaPrzychodow.DTOs;

namespace SystemUznawaniaPrzychodow.Services;

public interface IRevenueService
{
    Task<GetRevenueResponseDto> GetCurrentRevenueAsync(int? softwareId, string currency);
    Task<GetRevenueResponseDto> GetExpectedRevenueAsync(int? softwareId, string currency);
}