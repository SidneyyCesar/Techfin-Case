namespace Techfin.Transaction.Application.DTOs.Requests;

public sealed class CreateTransactionRequest
{
    public Guid idCliente { get; init; }

    public decimal ValorSimulado { get; init; }
}
