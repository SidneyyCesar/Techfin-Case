using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Techfin.Transaction.Application.DTOs.Requests;
using Techfin.Transaction.Application.Messaging;

namespace Techfin.Transaction.Api.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class TransacoesController(CreateTransactionHandler handler) : ControllerBase
{
    private readonly CreateTransactionHandler _handler = handler;

    [HttpPost]
    public async Task<IActionResult> Create([FromHeader(Name = "transacao-key")] string transacaoKey, CreateTransactionRequest request)
    {
        var authorization = HttpContext.Request.Headers.Authorization.ToString();

        string token = authorization.Replace("Bearer ", "");

        var command = new CreateTransactionCommand(request.idCliente, request.ValorSimulado, transacaoKey, token);

        var response = await _handler.HandleAsync(command);

        if (!response.Success)
        {
            if (response.StatusCode == HttpStatusCode.Accepted)
                return Accepted(response);

            return BadRequest(response);
        }
        return Created(string.Empty, response);
    }
}
