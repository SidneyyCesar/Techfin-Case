namespace Techfin.Case.Infra.Models;

public class UserSqlModel
{
    public string Guid { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }
}
