using Dapper;
using Techfin.Transaction.Domain.Repositories;
using Techfin.Transaction.Infra.Models;

namespace Techfin.Transaction.Infra.Repositories;

public sealed class TransactionRepository : ITransactionRepository
{
    private readonly IDbConnectionFactory _factory;

    public TransactionRepository(IDbConnectionFactory factory)
    {
        _factory = factory;
    }

    public async Task AddAsync(Transaction.Domain.Transaction transaction)
    {
        using var connection = _factory.CreateConnection();

        const string sql = @"INSERT INTO Transactions
                            (
                                Id,
                                CustomerId,
                                Amount,
                                CreatedAt
                            )
                            VALUES
                            (
                                @Id,
                                @CustomerId,
                                @Amount,
                                @CreatedAt
                            )
                            ";

        await connection.ExecuteAsync(
            sql,
            new
            {
                transaction.Id,
                transaction.CustomerId,
                transaction.Amount,
                transaction.CreatedAt
            });
    }

    public async Task<Domain.Transaction?> GetByIdAsync(Guid id)
    {
        using var connection = _factory.CreateConnection();

        const string sql = @"SELECT *
                            FROM Transactions
                            WHERE Id = @Id";

        var result = await connection.QueryFirstOrDefaultAsync<TransactionSqlModel>(sql, new { Id = id.ToString().ToUpper() });

        return result is null
            ? null
            : Map(result);
    }

    public async Task<IEnumerable<Domain.Transaction>> GetAllAsync()
    {
        using var connection = _factory.CreateConnection();

        const string sql = @"SELECT *
                                FROM Transactions
                                ORDER BY CreatedAt DESC";

        var result = await connection.QueryAsync<TransactionSqlModel>(sql);

        return result.Select(Map);
    }

    private static Domain.Transaction Map(TransactionSqlModel model)
    {
        return Domain.Transaction.Load(
            model.Id,
            model.CustomerId,
            model.Amount,
            model.CreatedAt);
    }
}
