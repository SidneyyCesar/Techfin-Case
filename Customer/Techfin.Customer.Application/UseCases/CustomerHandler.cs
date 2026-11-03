using System.Net;
using Techfin.Customer.Application.DTOs.Responses;
using Techfin.Customer.Application.Validator;
using Techfin.Customer.Domain.Repositories;
using Techfin.Shared;

namespace Techfin.Customer.Application.UseCases;

public sealed class CustomerHandler
{
    private readonly ICustomerRepository _repository;

    public CustomerHandler(ICustomerRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<CustomerResponse>> GetByIdAsync(Guid Id)
    {
        var response = await _repository.GetByIdAsync(Id);

        if (response is null)
        {
            return new Result<CustomerResponse>
            {
                Success = false,
                Error = "Não foi encontrado um cliente com o ID informado",
                StatusCode = HttpStatusCode.NotFound
            };
        }

        return new Result<CustomerResponse>
        {
            StatusCode = HttpStatusCode.OK,
            Success = true,
            Error = "Clientes consultados com sucesso",
            Data = new CustomerResponse()
            {
                Id = response.Id,
                CPF = response.CPF,
                Name = response.Name,
                LimitValue = response.LimitValue,
                CreatedAt = response.CreatedAt
            }
        };
    }

    public async Task<Result<List<CustomerResponse>>> ListAsync()
    {
        var response = await _repository.GetListAsync();

        return new Result<List<CustomerResponse>>
        {
            StatusCode = HttpStatusCode.OK,
            Success = true,
            Error = "Clientes consultados com sucesso",
            Data = response.Select(c => new CustomerResponse
            {
                Id = c.Id,
                CPF = c.CPF,
                Name = c.Name,
                LimitValue = c.LimitValue,
                CreatedAt = c.CreatedAt
            }).ToList()
        };
    }

    public async Task<Result<CustomerResponse>> IncludeAsync(CustomerInsertCommand command)
    {
        // Validação dos dados do request usando FluentValidation
        var validator = new CustomerInsertRequestValidator();
        var validationResult = await validator!.ValidateAsync(command);

        if (!validationResult.IsValid)
        {
            return new Result<CustomerResponse>
            {
                Success = false,
                Error = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)),
                StatusCode = HttpStatusCode.BadRequest
            };
        }

        var customerExists = await _repository.GetByCpfAsync(command.CPF);

        if (customerExists is not null)
        {
            return new Result<CustomerResponse>
            {
                Success = false,
                Error = "Já existe um cliente cadastrado com este CPF",
                StatusCode = HttpStatusCode.Conflict
            };
        }

        DateTime createdAt = DateTime.Now;

        var customer = new Domain.Customer(command.Name, command.CPF, command.LimitValue, createdAt);

        await _repository.AddAsync(customer);

        return new Result<CustomerResponse>
        {
            Success = true,
            StatusCode = HttpStatusCode.OK,
            Error = "Cliente cadastrado com sucesso",
            Data = new CustomerResponse()
            {
                Id = customer.Id,
                CPF = command.CPF,
                Name = command.Name,
                LimitValue = command.LimitValue,
                CreatedAt = createdAt,
            }
        };
    }

    public async Task<Result<CustomerResponse>> UpdateAsync(CustomerUpdateCommand command)
    {
        // Validação dos dados do request usando FluentValidation
        var validator = new CustomerUpdateRequestValidator();
        var validationResult = await validator!.ValidateAsync(command);

        if (!validationResult.IsValid)
        {
            return new Result<CustomerResponse>
            {
                Success = false,
                Error = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)),
                StatusCode = HttpStatusCode.BadRequest
            };
        }

        var customer =
            new Domain.Customer(Guid.Parse(command.Id),
                                command.Name,
                                command.CPF,
                                command.LimitValue);

        await _repository.UpdateAsync(customer);

        return new Result<CustomerResponse>
        {
            Success = true,
            StatusCode = HttpStatusCode.OK,
            Error = "Cliente atualizado com sucesso",
            Data = new CustomerResponse()
            {
                Id = Guid.Parse(command.Id),
                CPF = command.CPF,
                Name = command.Name,
                LimitValue = command.LimitValue
            }
        };
    }

    public async Task<Result<bool>> DeleteAsync(string Id)
    {
        if (string.IsNullOrEmpty(Id))
        {
            return new Result<bool>
            {
                Success = false,
                Error = "Não foi informado o ID do cliente",
                StatusCode = HttpStatusCode.NoContent
            };
        }

        await _repository.DeleteAsync(Id.ToUpper());

        return new Result<bool>
        {
            Success = true,
            Error = "Cliente deletado com sucesso",
            Data = true
        };
    }

}
