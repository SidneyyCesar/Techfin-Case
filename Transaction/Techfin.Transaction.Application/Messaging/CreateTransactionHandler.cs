using System.Net;
using Techfin.Contracts;
using Techfin.Shared;
using Techfin.Transaction.Application.DTOs;
using Techfin.Transaction.Domain.Repositories;
using Techfin.Transaction.Infra;
namespace Techfin.Transaction.Application.Messaging;

public sealed class CreateTransactionHandler
{
    private readonly ITransactionRepository _repository;
    private readonly IMessagePublisher _publisher;

    public CreateTransactionHandler(ITransactionRepository repository, IMessagePublisher publisher)
    {
        _repository = repository;
        _publisher = publisher;
    }

    public async Task<Result<CreateTransactionResponse>> HandleAsync(CreateTransactionCommand command)
    {
        if (command.CustomerId == Guid.Empty)
            return new Result<CreateTransactionResponse>()
            {
                StatusCode = HttpStatusCode.BadRequest,
                Error = "Cliente vinculado a transação não é válido."
            };

        if (string.IsNullOrEmpty(command.RequestId))
            return new Result<CreateTransactionResponse>()
            {
                StatusCode = HttpStatusCode.BadRequest,
                Error = "Identificador da transação não informado"
            };

        if (command.Amount < 0)
            return new Result<CreateTransactionResponse>()
            {
                StatusCode = HttpStatusCode.BadRequest,
                Error = "O valor da transação não pode ser negativo."
            };

        // Verificação se já existe uma transação com o mesmo IdempotencyKey
        var existing = await _repository.GetByRequestIdAsync(command.RequestId);

        if (existing is not null)
            return new Result<CreateTransactionResponse>()
            {
                StatusCode = HttpStatusCode.Accepted,
                Error = $"A Transação {command.RequestId} já foi executada e esta em processamento."
            };

        // IMPORTANTE
        // A validação do cliente vinculado a transação poderia ser feita aqui via uma chamada HTTP na api de clientes por exemplo
        // Estou omitindo esta validação por simplicidade no case

        var transaction = new Domain.Transaction(command.CustomerId, command.Amount, command.RequestId);

        // Persistencia da transação
        await _repository.AddAsync(transaction);

        // Publicação do evento na fila
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
                TransactionId = transaction.Id,
                RequestId = command.RequestId,
                TransactionDate = transaction.CreatedAt
            }
        };
    }
}
