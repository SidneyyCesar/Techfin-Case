namespace Techfin.Case.Application.UseCases.LoginUser;

public sealed record LoginUserCommand
(
    string Email,
    string Password
);
