using Dapper;
using System.Globalization;
using Techfin.Customer.Domain.Repositories;
using Techfin.Customer.Infra.Data;
using Techfin.Customer.Infra.Models;

namespace Techfin.Customer.Infra.Repository;

public class CustomerRepository(IDbConnectionFactory factory) : ICustomerRepository
{
    private readonly IDbConnectionFactory _factory = factory;

    public async Task<IEnumerable<Domain.Customer>> GetListAsync()
    {
        using var connection = _factory.CreateConnection();

        const string sql = @"SELECT Id,
                                    Name,
                                    CPF,
                                    CAST(LimitValue AS REAL) AS LimitValue,
                                    CreatedAt
                                FROM Customers
                                ORDER BY Name";

        var result = await connection.QueryAsync<CustomerSqlModel>(sql);

        return result.Select(Map);
    }

    public async Task<Domain.Customer?> GetByIdAsync(Guid id)
    {
        using var connection = _factory.CreateConnection();

        const string sql = @" SELECT *
                                FROM Customers
                                WHERE Upper(Id) = @Id";

        var result = await connection.QueryFirstOrDefaultAsync<CustomerSqlModel>(sql, new { Id = id.ToString().ToUpper() });

        return result is null ? null : Map(result);
    }

    public async Task AddAsync(Domain.Customer customer)
    {
        using var connection = _factory.CreateConnection();

        const string sql = @"INSERT INTO Customers(
                                Id,
                                Name,
                                CPF,
                                LimitValue,
                                CreatedAt
                            )
                            VALUES
                            (
                                @Id,
                                @Name,
                                @CPF,
                                @LimitValue,
                                @CreatedAt
                            )";

        await connection.ExecuteAsync(
            sql,
            new
            {
                customer.Id,
                customer.Name,
                customer.CPF,
                customer.LimitValue,
                customer.CreatedAt
            });
    }

    public async Task UpdateAsync(Domain.Customer customer, decimal AmountDebit = 0)
    {
        var updateValue = (customer.LimitValue - AmountDebit).ToString(CultureInfo.InvariantCulture);

        using var connection = _factory.CreateConnection();
        const string sql = @" UPDATE Customers
                            SET
                                Name = @Name,
                                CPF = @CPF,
                                LimitValue = LimitValue - @AmountDebit
                            WHERE Id = @Id
                            AND LimitValue >= @AmountDebit;";

        await connection.ExecuteAsync(
            sql,
            new
            {
                customer.Id,
                customer.Name,
                customer.CPF,
                customer.LimitValue,
                AmountDebit
            });
    }
    public async Task<Domain.Customer?> GetByCpfAsync(string CPF)
    {
        using var connection = _factory.CreateConnection();

        const string sql = @"SELECT *
                                FROM Customers
                                Where CPF = @CPF";

        var result = await connection.QueryFirstOrDefaultAsync<CustomerSqlModel>(sql, new { CPF });

        if (result is null)
            return null;

        return Map(result);

    }
    public async Task DeleteAsync(string id)
    {
        using var connection = _factory.CreateConnection();

        const string sql = @"DELETE 
                                FROM Customers
                            WHERE Id = @Id";

        await connection.ExecuteAsync(sql, new { Id = id.ToString() });
    }

    private static Domain.Customer Map(CustomerSqlModel customer)
    {
        var result = new Domain.Customer(
            Guid.Parse(customer.Id),
            customer.Name,
            customer.CPF,
            customer.LimitValue);

        result.CreatedAt = customer.CreatedAt;

        return result;
    }
}