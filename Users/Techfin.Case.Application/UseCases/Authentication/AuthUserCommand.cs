namespace Techfin.Case.Application.UseCases.Authentication;

public sealed record AuthUserCommand
(
    string Email,
    string Password
);
