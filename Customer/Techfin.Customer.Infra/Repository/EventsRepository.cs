using Dapper;
using Techfin.Customer.Domain.Entities;
using Techfin.Customer.Domain.Repositories;
using Techfin.Customer.Infra.Data;

namespace Techfin.Customer.Infra.Repository;

public class EventsRepository : IEventsRepository
{
    private readonly IDbConnectionFactory _factory;

    public EventsRepository(IDbConnectionFactory factory)
    {
        _factory = factory;
    }

    public async Task AddAsync(ProcessedEvents processdEvent)
    {
        using var connection = _factory.CreateConnection();

        const string sql = @"INSERT INTO Events(
                                Id,
                                Json,
                                ProcessedAt
                            )
                            VALUES
                            (
                                @Id,
                                @Json,
                                @ProcessedAt
                            )";

        await connection.ExecuteAsync(
            sql,
            new
            {
                processdEvent.Id,
                processdEvent.Json,
                processdEvent.ProcessedAt
            });
    }
}