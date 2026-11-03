using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Techfin.Customer.Application;
using Techfin.Customer.Application.DTOs.Requests;
using Techfin.Customer.Application.UseCases;

namespace Techfin.Customer.Controllers;

[ApiController]
[Route("[controller]")]
public class ClienteController : ControllerBase
{
    private readonly CustomerHandler _customerHandler;
    public ClienteController(CustomerHandler customerHandler)
    {
        _customerHandler = customerHandler;
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Insert(CustomerInsertRequest request)
    {
        CustomerInsertCommand command = new(request.Name, request.CPF, request.LimitValue);

        var response = await _customerHandler.IncludeAsync(command);

        if (!response.Success)
        {
            if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
                return Conflict(response.Error);

            return BadRequest(response.Error);
        }

        return Created(string.Empty, response);
    }

    [HttpPut]
    [Authorize]
    public async Task<IActionResult> Update(CustomerUpdateCommand request)
    {
        CustomerUpdateCommand command = new(request.Id, request.Name, request.CPF, request.LimitValue);

        var response = await _customerHandler.UpdateAsync(command);

        if (!response.Success)
            return BadRequest(response.Error);

        return Ok(response);
    }

    [HttpGet("{id:guid}")]
    [Authorize]
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
    [Authorize]
    public async Task<IActionResult> List()
    {
        var response = await _customerHandler.ListAsync();

        if (!response.Success)
            return BadRequest(response.Error);

        return Ok(response);
    }

    [HttpDelete]
    [Authorize]
    public async Task<IActionResult> Delete(string Id)
    {
        var response = await _customerHandler.DeleteAsync(Id);

        if (!response.Success)
            return BadRequest(response.Error);

        return Ok(response);
    }
}
