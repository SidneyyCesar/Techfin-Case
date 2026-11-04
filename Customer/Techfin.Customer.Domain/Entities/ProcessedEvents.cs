namespace Techfin.Customer.Domain.Entities;

public class ProcessedEvents
{
    public Guid Id { get; set; }
    public string Json { get; set; } = string.Empty;
    public DateTime ProcessedAt { get; set; }

}
