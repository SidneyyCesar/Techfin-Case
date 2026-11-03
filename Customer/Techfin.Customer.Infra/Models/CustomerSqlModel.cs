namespace Techfin.Customer.Infra.Models
{
    public class CustomerSqlModel
    {
        public string Id { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public string CPF { get; set; } = string.Empty;

        public decimal LimitValue { get; set; }

        public DateTime CreatedAt { get; set; }

    }
}
