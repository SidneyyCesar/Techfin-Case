namespace Techfin.Transaction.Application.DTOs.Responses;

public sealed class CreateTransactionResponse
{
    public Guid? IdTransacao { get; init; }
    public string ChaveTransacao { get; set; } = string.Empty;
    public DateTime? DataTransacao { get; set; }
    public string Status { get; set; } = string.Empty;
}
