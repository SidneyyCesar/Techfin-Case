using FluentValidation;

namespace Techfin.Customer.Application.Validator
{
    public class CustomerInsertRequestValidator : AbstractValidator<CustomerInsertCommand>
    {
        public CustomerInsertRequestValidator()
        {
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
