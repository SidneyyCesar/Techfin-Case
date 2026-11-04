using FluentValidation;
using Techfin.Customer.Application.DTOs.Requests;

namespace Techfin.Customer.Application.Validator
{
    public class CustomerInsertRequestValidator : AbstractValidator<CustomerInsertRequest>
    {
        public CustomerInsertRequestValidator()
        {
            RuleFor(x => x.Nome)
                .NotEmpty()
                .WithMessage("Nome do cliente não informado.")
                .MaximumLength(150);

            RuleFor(x => x.CPF)
                .NotEmpty()
                .WithMessage("Document CPF não informado.");

            RuleFor(x => x.ValorLimite)
                .GreaterThanOrEqualTo(0)
                .WithMessage("O valor do limite não pode ser negativo.");
        }
    }
}
