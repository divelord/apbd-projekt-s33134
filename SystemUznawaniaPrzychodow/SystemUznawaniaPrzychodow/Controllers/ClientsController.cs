using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SystemUznawaniaPrzychodow.DTOs;
using SystemUznawaniaPrzychodow.Exceptions;
using SystemUznawaniaPrzychodow.Services;

namespace SystemUznawaniaPrzychodow.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class ClientsController : ControllerBase
{
    private readonly IClientService _clientService;

    public ClientsController(IClientService clientService)
    {
        _clientService = clientService;
    }

    [Route("individual")]
    [HttpPost]
    public async Task<IActionResult> CreateIndividualClient(CreateIndividualClientDto dto)
    {
        try
        {
            await _clientService.CreateIndividualClientAsync(dto);

            return Created();
        }
        catch (ConflictException e)
        {
            return Conflict(e.Message);
        }
    }

    [Route("company")]
    [HttpPost]
    public async Task<IActionResult> CreateCompanyClient(CreateCompanyClientDto dto)
    {
        try
        {
            await _clientService.CreateCompanyClientAsync(dto);

            return Created();
        }
        catch (ConflictException e)
        {
            return Conflict(e.Message);
        }
    }

    [Route("individual/{id:int}")]
    [HttpPut]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateIndividualClient(int id, UpdateIndividualClientDto dto)
    {
        try
        {
            await _clientService.UpdateIndividualClientAsync(id, dto);

            return Ok();
        }
        catch (NotFoundException e)
        {
            return NotFound(e.Message);
        }
    }

    [Route("company/{id:int}")]
    [HttpPut]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateCompanyClient(int id, UpdateCompanyClientDto dto)
    {
        try
        {
            await _clientService.UpdateCompanyClientAsync(id, dto);

            return Ok();
        }
        catch (NotFoundException e)
        {
            return NotFound(e.Message);
        }
    }

    [Route("individual/{id:int}")]
    [HttpDelete]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteIndividualClient(int id)
    {
        try
        {
            await _clientService.DeleteIndividualClientAsync(id);

            return NoContent();
        }
        catch (NotFoundException e)
        {
            return NotFound(e.Message);
        }
        catch (ConflictException e)
        {
            return Conflict(e.Message);
        }
    }
}