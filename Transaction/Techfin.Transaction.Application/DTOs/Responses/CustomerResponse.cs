namespace Techfin.Transaction.Application.DTOs.Responses;

public sealed class CustomerResponse
{
    public Guid Id { get; init; }
    public string Nome { get; set; } = string.Empty;
    public decimal ValorLimite { get; set; }
}
