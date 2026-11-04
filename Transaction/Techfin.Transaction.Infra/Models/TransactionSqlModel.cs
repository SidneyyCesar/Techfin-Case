namespace Techfin.Transaction.Infra.Models;

public sealed class TransactionSqlModel
{
    public Guid Id { get; set; }

    public Guid CustomerId { get; set; }

    public decimal Amount { get; set; }

    public DateTime CreatedAt { get; set; }
}
