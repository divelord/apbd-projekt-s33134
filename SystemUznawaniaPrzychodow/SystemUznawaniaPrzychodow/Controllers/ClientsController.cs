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

    // POST /api/clients/individual
    [Route("individual")]
    [HttpPost]
    public async Task<IActionResult> CreateIndividualClient([FromBody] CreateIndividualClientDto dto)
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

    // POST /api/clients/company
    [Route("company")]
    [HttpPost]
    public async Task<IActionResult> CreateCompanyClient([FromBody] CreateCompanyClientDto dto)
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

    // PUT /api/clients/individual/{id}
    [Route("individual/{id:int}")]
    [HttpPut]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateIndividualClient(int id, [FromBody] UpdateIndividualClientDto dto)
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

    // PUT /api/clients/company/{id}
    [Route("company/{id:int}")]
    [HttpPut]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateCompanyClient(int id, [FromBody] UpdateCompanyClientDto dto)
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

    // DELETE /api/clients/individual/{id}
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