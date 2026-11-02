using Microsoft.AspNetCore.Mvc;
using Techfin.Case.Application.DTOs.Requests;
using Techfin.Case.Application.UseCases.RegisterUser;
using Techfin.Case.Application.Validators;

namespace TechFin.Case.Auth.Controllers;

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

        (Guid id, Notifications notification) = await _handler.HandleAsync(command);

        if (notification.hasError)
        {
            return BadRequest(notification.messageError);
        }

        return Created(string.Empty, new { UserId = id });
    }
}
