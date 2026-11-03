using FluentValidation;

namespace Techfin.Customer.Application.Validator
{
    public class CustomerUpdateRequestValidator : AbstractValidator<CustomerUpdateCommand>
    {
        public CustomerUpdateRequestValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage("O Payload não contêm um ID releacionado ao cliente.")
                .MaximumLength(150);

            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Nome do cliente não informado.")
                .MaximumLength(150);

            RuleFor(x => x.CPF)
                .NotEmpty()
                .WithMessage("Document CPF não informado.");

            RuleFor(x => x.LimitValue)
                .GreaterThanOrEqualTo(0)
                .WithMessage("O valor do limite não pode ser negativo.");
        }
    }
}
