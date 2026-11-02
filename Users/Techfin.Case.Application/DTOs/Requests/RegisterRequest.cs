namespace Techfin.Case.Application.DTOs.Requests;

public sealed class RegisterRequest
{
    public string Name { get; set; } = default!;

    public string Email { get; set; } = default!;

    public string Password { get; set; } = default!;
}
