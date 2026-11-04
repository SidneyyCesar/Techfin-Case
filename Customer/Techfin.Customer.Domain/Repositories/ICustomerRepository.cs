namespace Techfin.Customer.Domain.Repositories;

public interface ICustomerRepository
{
    Task<Customer?> GetByIdAsync(Guid id);

    Task<IEnumerable<Customer>> GetListAsync();

    Task AddAsync(Customer customer);

    Task UpdateAsync(Customer customer);

    Task DeleteAsync(string Id);

    Task<Domain.Customer?> GetByCpfAsync(string CPF);
}
