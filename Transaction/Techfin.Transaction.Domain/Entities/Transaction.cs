namespace Techfin.Transaction.Domain;

public sealed class Transaction
{
    public Guid Id { get; private set; }

    public Guid CustomerId { get; private set; }

    public decimal Amount { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public Transaction(Guid customerId, decimal amount)
    {
        Id = Guid.NewGuid();
        CustomerId = customerId;
        Amount = amount;
        CreatedAt = DateTime.UtcNow;
    }
    public static Transaction Load(Guid Id, Guid customerId, decimal amount, DateTime? createdAt)
    {
        return new Transaction(customerId, amount)
        {
            Id = Id,
            CreatedAt = createdAt ?? DateTime.UtcNow
        };
    }
}
