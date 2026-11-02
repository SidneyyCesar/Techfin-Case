namespace Techfin.Case.Application.DTOs.Responses;

public class LoginResponse
{
    public string AccessToken { get; init; } = string.Empty;

    public DateTime ExpiresAt { get; init; }
}
