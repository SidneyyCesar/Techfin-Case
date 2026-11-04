using Techfin.Customer.Domain.Entities;

namespace Techfin.Customer.Domain.Repositories;

public interface IEventsRepository
{
    Task AddAsync(ProcessedEvents processdEvent);
}
