namespace Techfin.Case.Application.UseCases.RegisterUser
{
    public record RegisterUserCommand
    (
        string Name,
        string Email,
        string Password
    );
}
