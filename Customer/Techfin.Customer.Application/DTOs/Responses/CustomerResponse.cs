namespace Techfin.Customer.Application.DTOs.Responses
{
    public class CustomerResponse
    {
        public Guid Id { get; set; }
        public string Name { get; init; } = string.Empty;
        public string CPF { get; set; } = string.Empty;
        public decimal LimitValue { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
