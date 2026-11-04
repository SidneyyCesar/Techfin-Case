using System.Data;

namespace Techfin.Transaction.Infra.Repositories;

public interface IDbConnectionFactory
{
    IDbConnection CreateConnection();
}
