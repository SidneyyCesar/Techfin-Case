using Microsoft.AspNetCore.Mvc;
using System.Net;
using Techfin.Case.Application.DTOs.Requests;
using Techfin.Case.Application.UseCases.Authentication;
using Techfin.Case.Application.UseCases.RefreshToken;

namespace TechFin.Case.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class AutenticacaoController(AuthUserHandler handler) : ControllerBase
{
    private readonly AuthUserHandler _handler = handler;

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var command = new AuthUserCommand(request.Email, request.Password);

        var result = await _handler.HandleAsync(command);

        if (!result.Success)
        {
            if (result.StatusCode == HttpStatusCode.Unauthorized)
                return Unauthorized(result);

            return BadRequest(result);
        }

        return Ok(result);
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh(RefreshTokenRequest request)
    {
        var result = await _handler.RefreshTokenAsync(new RefreshTokenCommand(request.AccessToken));

        if (!result.Success)
            return Unauthorized(result);

        return Ok(result);
    }
}
