namespace Techfin.Case.Domain.Entities;

public class User
{
    public User(string name, string email, string passwordHash)
    {
        Id = Guid.NewGuid();
        Name = name;
        Email = email;
        Password = passwordHash;
        CreatedAt = DateTime.UtcNow;
    }

    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public DateTime CreatedAt { get; set; }
}
