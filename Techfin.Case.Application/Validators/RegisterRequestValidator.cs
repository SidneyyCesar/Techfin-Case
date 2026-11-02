using FluentValidation;
using Techfin.Case.Application.UseCases.RegisterUser;

namespace Techfin.Case.Application.Validators;
public class RegisterUserValidator : AbstractValidator<RegisterUserCommand>
{
    public RegisterUserValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Nome do usuário não informado.")
            .MaximumLength(150);

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