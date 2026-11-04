namespace Techfin.Transaction.Application.DTOs;

public sealed class CreateTransactionRequest
{
    public Guid CustomerId { get; init; }

    public decimal Amount { get; init; }
}
