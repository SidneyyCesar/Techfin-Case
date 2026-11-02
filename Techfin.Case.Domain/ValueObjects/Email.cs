using System.Text.RegularExpressions;

namespace Techfin.Case.Domain.ValueObjects;

public sealed class Email
{
    public string Address { get; }
    public string? errorMessage { get; set; }

    public Email(string address)
    {
        if (string.IsNullOrWhiteSpace(address))
        {
            errorMessage = "O Email do usuário Techfin não foi informado.";
        }

        if (!IsValid(address))
        {
            errorMessage = "O Email do usuário Techfin não é valido.";
        }

        Address = address.Trim().ToLowerInvariant();
    }

    private static bool IsValid(string email)
    {
        return Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
    }

    public bool HasError()
    {
        return !string.IsNullOrEmpty(errorMessage);
    }


    //public override string ToString()
    //{
    //    return Address;
    //}

    //public override bool Equals(object? obj)
    //{
    //    return obj is Email other && Address == other.Address;
    //}

    //public override int GetHashCode()
    //{
    //    return Address.GetHashCode();
    //}

    //public static implicit operator string(Email email)
    //{
    //    return email.Address;
    //}

    //public static explicit operator Email(string address)
    //{
    //    return new(address);
    //}
}
