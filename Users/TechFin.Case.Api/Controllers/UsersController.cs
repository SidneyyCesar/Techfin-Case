using Microsoft.AspNetCore.Mvc;
using System.Net;
using Techfin.Case.Application.DTOs.Requests;
using Techfin.Case.Application.UseCases.RegisterUser;

namespace TechFin.Case.Users.Controllers;

[ApiController]
[Route("[controller]")]
public class UsersController : ControllerBase
{
    private readonly RegisterUserHandler _handler;

    public UsersController(RegisterUserHandler handler)
    {
        _handler = handler;
    }

    [HttpPost]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        RegisterUserCommand command = new(request.Name, request.Email, request.Password);

        var response = await _handler.HandleAsync(command);

        if (!response.Success)
        {
            if (response.StatusCode == HttpStatusCode.Conflict)
                return Conflict(response.Error);

            return BadRequest(response.Error);
        }

        return Created(string.Empty, response.Data);
    }
}
