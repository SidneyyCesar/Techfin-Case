namespace Techfin.Customer.Application.DTOs.Requests;

public class CustomerInsertRequest
{
    public string Nome { get; set; } = default!;
    public string CPF { get; set; } = default!;
    public decimal ValorLimite { get; set; } = default!;
}
