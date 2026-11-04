namespace Techfin.Transaction.Domain.Repositories;
public interface ITransactionRepository
{
    Task AddAsync(Transaction transaction);
    Task<Transaction?> GetByRequestIdAsync(string requestId);
    Task<bool> IsAvailableAsync();
}
