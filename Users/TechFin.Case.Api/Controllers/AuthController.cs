using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Security.Claims;
using Techfin.Case.Application.DTOs.Requests;
using Techfin.Case.Application.UseCases.LoginUser;
using Techfin.Case.Application.UseCases.RefreshToken;

namespace TechFin.Case.Users.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    private readonly LoginUserHandler _loginHandler;
    private readonly RefreshTokenHandler _refreshHandler;

    public AuthController(LoginUserHandler loginHandler, RefreshTokenHandler refreshHandler)
    {
        _loginHandler = loginHandler;
        _refreshHandler = refreshHandler;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var command = new LoginUserCommand(request.Email, request.Password);

        var result = await _loginHandler.HandleAsync(command);

        if (!result.Success)
        {
            if (result.StatusCode == HttpStatusCode.Unauthorized)
                return Unauthorized(result.Error);

            return BadRequest(result);
        }

        return Ok(result.Data);
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh(RefreshTokenRequest request)
    {
        var result = await _refreshHandler.HandleAsync(new RefreshTokenCommand(request.AccessToken));

        if (!result.Success)
            return Unauthorized(result);

        return Ok(result.Data);
    }

    [Authorize]
    [HttpGet()]
    public IActionResult Me()
    {
        return Ok(new
        {
            UserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value,
            Email = User.FindFirst(ClaimTypes.Email)?.Value,
            Name = User.FindFirst(ClaimTypes.Name)?.Value
        });
    }
}
