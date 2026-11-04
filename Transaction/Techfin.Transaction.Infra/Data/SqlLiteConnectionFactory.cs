using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using System.Data;
using Techfin.Transaction.Infra.Repositories;

namespace Techfin.Case.Infra.Data;
public sealed class SqliteConnectionFactory : IDbConnectionFactory
{
    private readonly string _connectionString;

    public SqliteConnectionFactory(IConfiguration configuration)
    {
        _connectionString =
            configuration.GetConnectionString("TransactionsDb")
            ?? throw new InvalidOperationException();
    }

    public IDbConnection CreateConnection()
    {
        return new SqliteConnection(_connectionString);
    }
}
