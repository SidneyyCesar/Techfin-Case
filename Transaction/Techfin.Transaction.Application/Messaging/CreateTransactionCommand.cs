namespace Techfin.Transaction.Application.Messaging;

public sealed record CreateTransactionCommand(Guid CustomerId, decimal Amount, string TransactionKey, string Token);
