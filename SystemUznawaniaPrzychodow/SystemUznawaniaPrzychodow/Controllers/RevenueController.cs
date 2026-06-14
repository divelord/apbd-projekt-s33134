using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SystemUznawaniaPrzychodow.DTOs;
using SystemUznawaniaPrzychodow.Exceptions;
using SystemUznawaniaPrzychodow.Services;

namespace SystemUznawaniaPrzychodow.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class RevenueController : ControllerBase
{
    private readonly IRevenueService _revenueService;

    public RevenueController(IRevenueService revenueService)
    {
        _revenueService = revenueService;
    }

    [Route("current")]
    [HttpGet]
    public async Task<IActionResult> GetCurrentRevenue([FromQuery] GetRevenueDto dto)
    {
        try
        {
            var revenue = await _revenueService.GetCurrentRevenueAsync(dto.SoftwareId, dto.Currency);

            return Ok(revenue);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [Route("expected")]
    [HttpGet]
    public async Task<IActionResult> GetExpectedRevenue([FromQuery] GetRevenueDto dto)
    {
        try
        {
            var revenue = await _revenueService.GetExpectedRevenueAsync(dto.SoftwareId, dto.Currency);

            return Ok(revenue);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
}