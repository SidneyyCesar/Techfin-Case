using Techfin.Case.Domain.Entities;

namespace Techfin.Case.Application.Interfaces;

public interface IJwtProvider
{
    string GenerateToken(User user);
}
