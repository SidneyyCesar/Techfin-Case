namespace Techfin.Customer.Application.DTOs.Requests;

public record CustomerUpdateRequest
(
    string Id,
    string? Nome,
    string? CPF,
    decimal ValorLimite
);
