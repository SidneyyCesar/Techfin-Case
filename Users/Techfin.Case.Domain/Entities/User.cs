namespace Techfin.Case.Domain.Entities;
public class User(Guid id, string name, string email, string password, DateTime createdAt)
{
    public Guid Id { get; private set; } = id;
    public string Name { get; private set; } = name;
    public string Email { get; private set; } = email;
    public string Password { get; private set; } = password;
    public DateTime CreatedAt { get; private set; } = createdAt;
}
