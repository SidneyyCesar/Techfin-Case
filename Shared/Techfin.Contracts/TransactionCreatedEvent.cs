namespace Techfin.Contracts;

public sealed record TransactionCreatedEvent
{
    public Guid TransactionId { get; init; }

    public Guid CustomerId { get; init; }

    public decimal Amount { get; init; }

    public DateTime CreatedAt { get; init; }
}
