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
                                RequestId,
                                CustomerId,
                                Amount,
                                CreatedAt,
                                Status
                            )
                            VALUES
                            (
                                @Id,
                                @RequestId,                         
                                @CustomerId,
                                @Amount,
                                @CreatedAt,
                                'Aprovado.'       
                            )
                            ";

        await connection.ExecuteAsync(
            sql,
            new
            {
                transaction.Id,
                transaction.RequestId,
                transaction.CustomerId,
                transaction.Amount,
                transaction.CreatedAt
            });
    }

    public async Task<Domain.Transaction?> GetByRequestIdAsync(string requestId)
    {
        using var connection = _factory.CreateConnection();

        const string sql = @"SELECT *
                            FROM Transactions
                            WHERE RequestId = @RequestId";

        var result = await connection.QueryFirstOrDefaultAsync<TransactionSqlModel>(sql, new { RequestId = requestId });

        return result is null
            ? null
            : Map(result);
    }

    public async Task<bool> IsAvailableAsync()
    {
        try
        {
            using var _connection = _factory.CreateConnection();

            await _connection.ExecuteScalarAsync<int>("SELECT 1 FROM Transactions");

            return true;
        }
        catch
        {
            return false;
        }
    }

    private static Domain.Transaction Map(TransactionSqlModel model)
    {
        return new Domain.Transaction(Guid.Parse(model.CustomerId), model.Amount, model.RequestId)
        {
            Id = Guid.Parse(model.Id),
            CreatedAt = model.CreatedAt
        };
    }
}
