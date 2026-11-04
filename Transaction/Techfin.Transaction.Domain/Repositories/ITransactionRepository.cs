namespace Techfin.Transaction.Domain.Repositories;
public interface ITransactionRepository
{
    Task AddAsync(Transaction transaction);

    Task<Transaction?> GetByIdAsync(Guid id);

    Task<IEnumerable<Transaction>> GetAllAsync();
}
