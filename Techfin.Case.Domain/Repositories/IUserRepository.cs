using Techfin.Case.Domain.Entities;

namespace Techfin.Case.Domain.Repositories;

public interface IUserRepository
{

    Task CreateAsync(User user);

    Task UpdateAsync(User user);
}
