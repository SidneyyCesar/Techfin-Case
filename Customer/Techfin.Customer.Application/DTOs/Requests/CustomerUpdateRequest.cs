namespace Techfin.Customer.Application.DTOs.Requests;

public class CustomerUpdateRequest
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = default!;
    public string CPF { get; set; } = default!;
    public decimal LimitValue { get; set; } = default!;
}
