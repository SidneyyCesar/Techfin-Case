namespace Techfin.Case.Application.Interfaces;

public interface IPasswordVerify
{
    string Hash(string password);

    bool Verify(string password, string hash);
}
