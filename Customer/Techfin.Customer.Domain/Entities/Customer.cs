namespace Techfin.Customer.Domain;
public class Customer
{
    public Customer(Guid id, string name, string cpf, decimal limiteValue)
    {
        Id = id;
        Name = name;
        CPF = cpf;
        LimitValue = limiteValue;
    }
    public Customer(string name, string cpf, decimal limitValue, DateTime createdAt)
    {
        Id = Guid.NewGuid();
        Name = name;
        CPF = cpf;
        LimitValue = limitValue;
        CreatedAt = createdAt;
    }

    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public string CPF { get; private set; }
    public decimal LimitValue { get; set; }
    public DateTime CreatedAt { get; private set; }
}
