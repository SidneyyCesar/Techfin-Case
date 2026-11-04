namespace Techfin.Transaction.Application.DTOs;

public sealed class CreateTransactionResponse
{
    public Guid TransactionId { get; init; }
    public string RequestId { get; set; } = string.Empty;
    public DateTime? TransactionDate { get; set; }
}
