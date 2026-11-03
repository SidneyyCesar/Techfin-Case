using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace Techfin.Customer.Infra.Data;

public sealed class SqliteConnectionFactory : IDbConnectionFactory
{
    private readonly string _connectionString;

    public SqliteConnectionFactory(IConfiguration configuration)
    {
        _connectionString =
            configuration.GetConnectionString("CustomersDb")
            ?? throw new InvalidOperationException();
    }

    public IDbConnection CreateConnection()
    {
        return new SqliteConnection(_connectionString);
    }
}
