using Techfin.Case.Application.Interfaces;

namespace Techfin.Case.Infra.Security;

public sealed class PasswordVerify : IPasswordVerify
{
    public string Hash(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }
    public bool Verify(string password, string hash)
    {
        return BCrypt.Net.BCrypt.Verify(password, hash);
    }
}
