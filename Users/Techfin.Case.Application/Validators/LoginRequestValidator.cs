using FluentValidation;
using Techfin.Case.Application.UseCases.LoginUser;

namespace Techfin.Case.Application.Validators;
public sealed class LoginRequestValidator : AbstractValidator<LoginUserCommand>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email não informado.")
            .EmailAddress()
            .WithMessage("Formato de email inválido.");

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Senha não informada.")
            .MinimumLength(6)
            .WithMessage("A senha precisa conter pelo menos 6 caracteres.");
    }
}
