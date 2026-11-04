using Techfin.Customer.Application.DTOs.Requests;
using Techfin.Customer.Application.DTOs.Responses;
using Techfin.Shared;

namespace Techfin.Customer.Application.Interfaces;
public interface ICustomerService
{
    Task<Result<CustomerResponse>> GetByIdAsync(Guid Id);
    Task<Result<List<CustomerResponse>>> ListAsync();
    Task<Result<CustomerResponse>> IncludeAsync(CustomerInsertRequest request);
    Task<Result<CustomerResponse>> UpdateAsync(CustomerUpdateRequest request);
    Task<Result<bool?>> DeleteAsync(string Id);
}
