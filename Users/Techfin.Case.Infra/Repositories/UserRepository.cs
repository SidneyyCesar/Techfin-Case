using Dapper;
using Techfin.Case.Domain.Entities;
using Techfin.Case.Domain.Repositories;
using Techfin.Case.Infra.Data;
using Techfin.Case.Infra.Models;

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

    public async Task<User?> GetByEmailAsync(string email)
    {
        using var connection = _factory.CreateConnection();

        const string sql = @"SELECT
                            Id as GUID,
                            Name,
                            Email,
                            Password,
                            CreatedAt
                        FROM Users
                        WHERE Email = @Email";

        var result = await connection.QueryFirstOrDefaultAsync<UserSqlModel>(sql, new { Email = email });

        if (result is null)
            return null;

        return Map(result);
    }

    public async Task<User?> GetByIdAsync(Guid id)
    {
        using var connection = _factory.CreateConnection();

        const string sql = @"SELECT
                                Id Guid,
                                Name,
                                Email,
                                Password,
                                CreatedAt
                            FROM Users
                            WHERE Id = @Id";

        string GUID = id.ToString()!.ToUpper();

        var result = await connection.QueryFirstOrDefaultAsync<UserSqlModel>(sql, new { Id = GUID });

        if (result is null)
            return null;

        return Map(result);
    }
    public Task UpdateAsync(User user)
    {
        throw new NotImplementedException();
    }

    private static User Map(UserSqlModel model)
    {
        return new User(Guid.Parse(model.Guid),
                        model.Name,
                        model.Email,
                        model.Password,
                        model.CreatedAt);
    }
}
