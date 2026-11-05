using FluentValidation;
using Techfin.Customer.Application.DTOs.Requests;

namespace Techfin.Customer.Application.Validator;

public class CustomerUpdateRequestValidator : AbstractValidator<CustomerUpdateRequest>
{
    public CustomerUpdateRequestValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("O Payload não contêm um ID releacionado ao cliente.")
            .MaximumLength(150);

        RuleFor(x => x.ValorLimite)
            .GreaterThanOrEqualTo(0)
            .WithMessage("O valor do limite não pode ser negativo.");
    }
}
