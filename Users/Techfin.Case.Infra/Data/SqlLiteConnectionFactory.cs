using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace Techfin.Case.Infra.Data;
public sealed class SqlLiteConnectionFactory : IDbConnectionFactory
{
    private readonly string _connectionString;

    public SqlLiteConnectionFactory(IConfiguration configuration)
    {
        _connectionString =
            configuration.GetConnectionString("UsersDb")
            ?? throw new InvalidOperationException();
    }

    public IDbConnection CreateConnection()
    {
        return new SqliteConnection(_connectionString);
    }
}
