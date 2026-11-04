using System.Data;

namespace Techfin.Customer.Infra.Data;

public interface IDbConnectionFactory
{
    IDbConnection CreateConnection();
}
