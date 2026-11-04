namespace Techfin.Case.Domain.Entities;
public class User
{
    public User(Guid id, string name, string email, string password, DateTime createdAt)
    {
        Id = id;
        Name = name;
        Email = email;
        Password = password;
        CreatedAt = createdAt;
    }

    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public string Email { get; private set; }
    public string Password { get; private set; }
    public DateTime CreatedAt { get; private set; }
}
