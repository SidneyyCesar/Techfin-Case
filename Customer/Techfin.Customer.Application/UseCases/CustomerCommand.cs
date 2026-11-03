namespace Techfin.Customer.Application;

public record CustomerInsertCommand
(
    string Name,
    string CPF,
    decimal LimitValue
);
public record CustomerUpdateCommand
(
    string Id,
    string Name,
    string CPF,
    decimal LimitValue
);