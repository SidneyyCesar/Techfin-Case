namespace Techfin.Transaction.Domain;
public class Transaction
{
    public Guid Id { get; set; }
    public string RequestId { get; private set; }
    public decimal Amount { get; private set; }
    public Guid CustomerId { get; set; }
    public DateTime CreatedAt { get; set; }

    public Transaction(Guid customerId, decimal amount, string requestId)
    {
        Id = Guid.NewGuid();
        CustomerId = customerId;
        Amount = amount;
        RequestId = requestId;
        CreatedAt = DateTime.UtcNow;
    }
}
