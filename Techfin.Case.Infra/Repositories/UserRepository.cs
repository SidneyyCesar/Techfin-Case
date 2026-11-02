using Dapper;
using Techfin.Case.Domain.Entities;
using Techfin.Case.Domain.Repositories;
using Techfin.Case.Infra.Data;

namespace Techfin.Case.Infra.Repositories;

public sealed class UserRepository : IUserRepository
{
    private readonly IDbConnectionFactory _factory;

    public UserRepository(IDbConnectionFactory factory)
    {
        _factory = factory;
    }

    public async Task AddAsync(User user)
    {
        using System.Data.IDbConnection connection = _factory.CreateConnection();

        const string sql = @"INSERT INTO Users(
                                    Id,
                                    Name,
                                    Email,
                                    Password,
                                    CreatedAt
                                )
                                VALUES
                                (
                                    @Id,
                                    @Name,
                                    @Email,
                                    @Password,
                                    @CreatedAt
                                )";

        _ = await connection.ExecuteAsync(
            sql,
            new
            {
                user.Id,
                user.Name,
                user.Email,
                user.Password,
                user.CreatedAt
            });
    }

    public async Task<bool> ExistsByEmailAsync(string email)
    {
        using System.Data.IDbConnection connection = _factory.CreateConnection();

        const string sql = @"SELECT COUNT(1) FROM Users WHERE Email = @Email";

        int count = await connection.ExecuteScalarAsync<int>(sql, new { Email = email });

        return count > 0;
    }

    public Task<User?> GetByEmailAsync(string email)
    {
        throw new NotImplementedException();
    }

    public Task<User?> GetByIdAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task UpdateAsync(User user)
    {
        throw new NotImplementedException();
    }
}
