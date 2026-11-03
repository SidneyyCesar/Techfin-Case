namespace Techfin.Customer.Application.DTOs.Requests;

public class CustomerInsertRequest
{
    public string Name { get; set; } = default!;
    public string CPF { get; set; } = default!;
    public decimal LimitValue { get; set; } = default!;
}
