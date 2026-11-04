namespace Techfin.Case.Application.DTOs.Requests;

public sealed class RefreshTokenRequest
{
    public string AccessToken { get; init; } = string.Empty;
}
