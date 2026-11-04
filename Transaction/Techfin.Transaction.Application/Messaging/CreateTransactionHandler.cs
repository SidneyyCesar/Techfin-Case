using System.Net;
using Techfin.Contracts;
using Techfin.Shared;
using Techfin.Transaction.Application.DTOs;
using Techfin.Transaction.Domain.Repositories;
using Techfin.Transaction.Infra;

namespace Techfin.Transaction.Application.Messaging;

public sealed class CreateTransactionHandler
{
    private readonly ITransactionRepository _transactionRepository;
    private readonly IMessagePublisher _publisher;

    public CreateTransactionHandler(ITransactionRepository repository, IMessagePublisher publisher)
    {
        _transactionRepository = repository;
        _publisher = publisher;
    }

    public async Task<Result<CreateTransactionResponse>> HandleAsync(CreateTransactionCommand command)
    {
        if (command.CustomerId == Guid.Empty)
            return new Result<CreateTransactionResponse>()
            {
                Success = false,
                StatusCode = HttpStatusCode.BadRequest,
                Error = "Cliente vinculado a transação não é válido."
            };

        // IMPORTANTE
        // A validação do cliente vinculado a transação deveria ser feita via uma chamada HTTP na api de clientes
        // Estou omitindo esta validação por simplicidade no case

        if (command.Amount < 0)
            return new Result<CreateTransactionResponse>()
            {
                Success = false,
                StatusCode = HttpStatusCode.BadRequest,
                Error = "O valor da transação não pode ser negativo."
            };

        var transaction = new Domain.Transaction(command.CustomerId, command.Amount);

        await _transactionRepository.AddAsync(transaction);

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
                TransactionId = transaction.Id
            }
        };
    }
}
