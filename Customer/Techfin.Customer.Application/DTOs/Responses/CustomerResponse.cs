namespace Techfin.Customer.Application.DTOs.Responses
{
    public class CustomerResponse
    {
        public Guid Id { get; set; }
        public string Nome { get; init; } = string.Empty;
        public string CPF { get; set; } = string.Empty;
        public decimal ValorLimite { get; set; }
        public DateTime DataCriacao { get; set; }
    }
}
