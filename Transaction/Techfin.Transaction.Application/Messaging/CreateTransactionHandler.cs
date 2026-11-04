using System.Net;
using System.Text.Json;
using Techfin.Contracts;
using Techfin.Shared;
using Techfin.Transaction.Application.DTOs.Responses;
using Techfin.Transaction.Domain.Repositories;
using Techfin.Transaction.Infra;

namespace Techfin.Transaction.Application.Messaging;

public sealed class CreateTransactionHandler(ITransactionRepository repository, IMessagePublisher publisher, HttpClient httpClient)
{
    private readonly ITransactionRepository _repository = repository;
    private readonly IMessagePublisher _publisher = publisher;
    private readonly HttpClient _httpClient = httpClient;

    /// <summary>
    /// A criação de uma transação funciona de maneira atomica, ou seja, se algum dos serviços estiver indisponível, a transação não será criada.
    /// </summary>
    /// <param name="command">dados da transação</param>
    public async Task<Result<CreateTransactionResponse>> HandleAsync(CreateTransactionCommand command)
    {
        var validation = await RequestValidation(command);

        if (!validation.Success)
            return validation;

        var transaction = new Domain.Transaction(command.CustomerId, command.Amount, command.TransactionKey);

        await _repository.AddAsync(transaction);

        await _publisher.PublishAsync(
            new TransactionCreatedEvent
            {
                TransactionId = transaction.Id,
                CustomerId = transaction.CustomerId,
                Amount = transaction.Amount,
                CreatedAt = transaction.CreatedAt
            });

        return new Result<CreateTransactionResponse>()
        {
            Success = true,
            StatusCode = HttpStatusCode.OK,
            Data = new CreateTransactionResponse
            {
                IdTransacao = transaction.Id,
                ChaveTransacao = command.TransactionKey,
                DataTransacao = transaction.CreatedAt,
                Status = "Aprovado"
            }
        };
    }

    #region Validação de Requisição

    private async Task<Result<CreateTransactionResponse>> RequestValidation(CreateTransactionCommand command)
    {
        string messageError = string.Empty;

        if (command.CustomerId == Guid.Empty)
            messageError = "Cliente vinculado a transação não é válido.";

        if (string.IsNullOrEmpty(command.TransactionKey))
            messageError = "Identificador da transação não informado.";

        if (command.Amount < 0)
            messageError = "O valor da transação não pode ser negativo.";

        var customerResponse = await RequestCustomerApiAsync(command);
        if (!customerResponse.Success)
        {
            return new Result<CreateTransactionResponse>()
            {
                Message = "Não foi possível localizar o cliente desta transação",
                Data = new CreateTransactionResponse()
                {
                    ChaveTransacao = command.TransactionKey,
                    Status = "Negado"
                }
            };
        }

        if (command.Amount > customerResponse.Data!.ValorLimite)
            messageError = $"O valor da transação é superior ao limite disponível - transação: {command.Amount} | limite do cliente {customerResponse.Data.ValorLimite}.";

        bool rabbitIsAvaliable = await _publisher.IsAvailableAsync();
        bool sqlLiteIsAvaliable = await _repository.IsAvailableAsync();

        if (!rabbitIsAvaliable || !sqlLiteIsAvaliable)
        {
            messageError = "Serviço indisponível no momento, tente novamente mais tarde.";
        }

        var existing = await _repository.GetByRequestIdAsync(command.TransactionKey);
        if (existing is not null)
        {
            messageError = $"A Transação {command.TransactionKey} já foi executada anteriormente.";
        }

        if (!string.IsNullOrEmpty(messageError))
            return new Result<CreateTransactionResponse>()
            {
                Message = messageError,
                Data = new CreateTransactionResponse()
                {
                    ChaveTransacao = command.TransactionKey,
                    Status = "Negado"
                }
            };

        return new Result<CreateTransactionResponse>() { Success = true };
    }

    private async Task<Result<CustomerResponse>> RequestCustomerApiAsync(CreateTransactionCommand command)
    {
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {command.Token}");

        var response = await _httpClient.GetAsync($"https://localhost:4000/clientes/{command.CustomerId}");

        if (response.IsSuccessStatusCode)
        {
            string jsonResponse = await response.Content.ReadAsStringAsync();

            using JsonDocument doc = JsonDocument.Parse(jsonResponse);
            JsonElement dataElement = doc.RootElement.GetProperty("data");

            var customer = JsonSerializer.Deserialize<CustomerResponse>(dataElement.GetRawText());

            if (customer != null)
            {
                return new Result<CustomerResponse>()
                {
                    Success = true,
                    StatusCode = HttpStatusCode.OK,
                    Data = new CustomerResponse()
                    {
                        Id = customer.Id,
                        Nome = customer.Nome,
                        ValorLimite = customer.ValorLimite
                    }
                };
            }
        }

        return new Result<CustomerResponse>()
        {
            Success = false,
            StatusCode = HttpStatusCode.BadRequest,
            Message = "Cliente vinculado a transação não encontrado."
        };

    }

    #endregion
}
