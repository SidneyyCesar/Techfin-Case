using System.Data;

namespace Techfin.Case.Infra.Data;
public interface IDbConnectionFactory
{
    IDbConnection CreateConnection();
}
