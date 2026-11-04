using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Techfin.Customer.Application.DTOs.Requests;
using Techfin.Customer.Application.Interfaces;

namespace Techfin.Customer.Api.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class ClientesController(ICustomerService customerHandler) : ControllerBase
{
    private readonly ICustomerService _customerHandler = customerHandler;

    [HttpPost]
    public async Task<IActionResult> Insert(CustomerInsertRequest request)
    {
        var response = await _customerHandler.IncludeAsync(request);

        if (!response.Success)
        {
            if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
                return Conflict(response);

            return BadRequest(response);
        }

        return Created(string.Empty, response);
    }

    [HttpPut]
    public async Task<IActionResult> Update(CustomerUpdateRequest request)
    {
        CustomerUpdateRequest command = new(request.Id, request.Nome, request.CPF, request.ValorLimite);

        var response = await _customerHandler.UpdateAsync(command);

        if (!response.Success)
        {
            if (response.StatusCode == HttpStatusCode.NotFound)
                return NotFound(response);

            return BadRequest(response);
        }

        return Ok(response);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid Id)
    {
        var response = await _customerHandler.GetByIdAsync(Id);

        if (!response.Success)
        {
            if (response.StatusCode == HttpStatusCode.NotFound)
                return NotFound(response);

            return BadRequest(response);
        }

        return Ok(response);
    }

    [HttpGet]
    public async Task<IActionResult> List()
    {
        var response = await _customerHandler.ListAsync();

        if (!response.Success)
            return BadRequest(response);

        return Ok(response);
    }

    [HttpDelete]
    public async Task<IActionResult> Delete(string Id)
    {
        var response = await _customerHandler.DeleteAsync(Id);

        if (!response.Success)
        {
            if (response.StatusCode == HttpStatusCode.NotFound)
                return NotFound(response);

            return BadRequest(response);
        }

        return Ok(response);
    }
}
