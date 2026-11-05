using Microsoft.Extensions.Caching.Memory;
using System.Net;
using Techfin.Customer.Application.DTOs.Requests;
using Techfin.Customer.Application.DTOs.Responses;
using Techfin.Customer.Application.Interfaces;
using Techfin.Customer.Application.Validator;
using Techfin.Customer.Domain.Repositories;
using Techfin.Shared;

namespace Techfin.Customer.Application.Services;
public sealed class CustomerService(ICustomerRepository repository, IMemoryCache cache) : ICustomerService
{
    private readonly ICustomerRepository _repository = repository;
    private readonly IMemoryCache _cache = cache;
    const string cacheKey = "customers_list";

    public async Task<Result<CustomerResponse>> GetByIdAsync(Guid Id)
    {
        var response = await _repository.GetByIdAsync(Id);

        if (response is null)
        {
            return new Result<CustomerResponse>
            {
                Success = false,
                Message = "Não foi encontrado um cliente com o ID informado",
                StatusCode = HttpStatusCode.NotFound
            };
        }

        return new Result<CustomerResponse>
        {
            StatusCode = HttpStatusCode.OK,
            Success = true,
            Message = "Clientes consultados com sucesso",
            Data = new CustomerResponse()
            {
                Id = response.Id,
                CPF = response.CPF,
                Nome = response.Name,
                ValorLimite = response.LimitValue,
                DataCriacao = response.CreatedAt
            }
        };
    }

    public async Task<Result<List<CustomerResponse>>> ListAsync()
    {
        // Tenta primeiramente obter dados do cache caso contrário acessa o repositório
        var customers = await _cache.GetOrCreateAsync(cacheKey,
        async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1);

            var response = await _repository.GetListAsync();

            return response.Select(c => new CustomerResponse
            {
                Id = c.Id,
                CPF = c.CPF,
                Nome = c.Name,
                ValorLimite = c.LimitValue,
                DataCriacao = c.CreatedAt
            }).ToList();
        });

        return new Result<List<CustomerResponse>>
        {
            StatusCode = HttpStatusCode.OK,
            Success = true,
            Message = "Clientes consultados com sucesso",
            Data = customers
        };
    }

    public async Task<Result<CustomerResponse>> IncludeAsync(CustomerInsertRequest request)
    {
        var validator = new CustomerInsertRequestValidator();
        var validationResult = await validator!.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            return new Result<CustomerResponse>
            {
                Success = false,
                Message = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)),
                StatusCode = HttpStatusCode.BadRequest
            };
        }

        var customerExists = await _repository.GetByCpfAsync(request.CPF);

        if (customerExists is not null)
        {
            return new Result<CustomerResponse>
            {
                Success = false,
                Message = "Já existe um cliente cadastrado com este CPF",
                StatusCode = HttpStatusCode.Conflict
            };
        }

        DateTime createdAt = DateTime.Now;

        var customer = new Domain.Customer(request.Nome, request.CPF, request.ValorLimite, createdAt);

        await _repository.AddAsync(customer);

        return new Result<CustomerResponse>
        {
            Success = true,
            StatusCode = HttpStatusCode.OK,
            Message = "Cliente cadastrado com sucesso",
            Data = new CustomerResponse()
            {
                Id = customer.Id,
                CPF = request.CPF,
                Nome = request.Nome,
                ValorLimite = request.ValorLimite,
                DataCriacao = createdAt
            }
        };
    }

    public async Task<Result<CustomerResponse>> UpdateAsync(CustomerUpdateRequest command)
    {
        var validator = new CustomerUpdateRequestValidator();
        var validationResult = await validator!.ValidateAsync(command);

        if (!validationResult.IsValid)
        {
            return new Result<CustomerResponse>
            {
                Success = false,
                Message = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)),
                StatusCode = HttpStatusCode.BadRequest
            };
        }

        var existsCustomer = await _repository.GetByIdAsync(Guid.Parse(command.Id));
        if (existsCustomer is null)
            return new Result<CustomerResponse>
            {
                Success = false,
                Message = "Cliente não encontrado",
                StatusCode = HttpStatusCode.NotFound
            };

        var customer = new Domain.Customer(Guid.Parse(command.Id),
                                            !string.IsNullOrEmpty(command.Nome) ? command.Nome : existsCustomer.Name,
                                            !string.IsNullOrEmpty(command.CPF) ? command.CPF : existsCustomer.CPF,
                                            command.ValorLimite > 0 ? command.ValorLimite : existsCustomer.LimitValue);

        await _repository.UpdateAsync(customer);

        return new Result<CustomerResponse>
        {
            Success = true,
            StatusCode = HttpStatusCode.OK,
            Message = "Cliente atualizado com sucesso",
            Data = new CustomerResponse()
            {
                Id = Guid.Parse(command.Id),
                CPF = !string.IsNullOrEmpty(command.CPF) ? command.CPF : existsCustomer.CPF,
                Nome = !string.IsNullOrEmpty(command.Nome) ? command.Nome : existsCustomer.Name,
                ValorLimite = command.ValorLimite
            }
        };
    }

    public async Task<Result<bool?>> DeleteAsync(string Id)
    {
        if (string.IsNullOrEmpty(Id))
        {
            return new Result<bool?>
            {
                Success = false,
                Message = "Não foi informado o ID do cliente",
                StatusCode = HttpStatusCode.NoContent
            };
        }

        var existsCustomer = await _repository.GetByIdAsync(Guid.Parse(Id));

        if (existsCustomer is null)
            return new Result<bool?>
            {
                Success = false,
                Message = "Cliente não encontrado",
                StatusCode = HttpStatusCode.NotFound
            };

        await _repository.DeleteAsync(Id.ToUpper());

        return new Result<bool?>
        {
            Success = true,
            Message = "Cliente deletado com sucesso",
            Data = true
        };
    }
}
