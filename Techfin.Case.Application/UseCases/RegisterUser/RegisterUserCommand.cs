namespace Techfin.Case.Application.UseCases.RegisterUser
{
    public sealed record RegisterUserCommand
    (
        string Name,
        string Email,
        string Password
    );
}
