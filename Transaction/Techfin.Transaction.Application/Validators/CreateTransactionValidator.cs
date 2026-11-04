using FluentValidation;
using Techfin.Transaction.Application.Messaging;

namespace Techfin.Transaction.Application.Validators;

public sealed class CreateTransactionValidator : AbstractValidator<CreateTransactionCommand>
{
    public CreateTransactionValidator()
    {
        RuleFor(x => x.CustomerId)
            .NotEmpty().
            WithMessage("Cliente não informado na transação");

        RuleFor(x => x.Amount)
            .GreaterThan(0)
            .WithMessage("O valor da transação não pode ser negativo");
    }
}
