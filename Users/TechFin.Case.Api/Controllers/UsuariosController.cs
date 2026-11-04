using Microsoft.AspNetCore.Mvc;
using System.Net;
using Techfin.Case.Application.DTOs.Requests;
using Techfin.Case.Application.UseCases.RegisterUser;

namespace TechFin.Case.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class UsuariosController(RegisterUserHandler userHandler) : ControllerBase
{
    private readonly RegisterUserHandler _userHandler = userHandler;

    [HttpPost]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        RegisterUserCommand command = new(request.Name, request.Email, request.Password);

        var response = await _userHandler.HandleAsync(command);

        if (!response.Success)
        {
            if (response.StatusCode == HttpStatusCode.Conflict)
                return Conflict(response);

            return BadRequest(response);
        }

        return Created(string.Empty, response);
    }
}
