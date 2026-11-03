using System.Net;
using System.Security.Claims;
using Techfin.Case.Application.DTOs.Responses;
using Techfin.Case.Application.Interfaces;
using Techfin.Case.Domain.Repositories;
using Techfin.Shared;

namespace Techfin.Case.Application.UseCases.RefreshToken;

public sealed class RefreshTokenHandler
{
    private readonly IJwtProvider _jwtProvider;
    private readonly IUserRepository _userRepository;

    public RefreshTokenHandler(IJwtProvider jwtProvider, IUserRepository userRepository)
    {
        _jwtProvider = jwtProvider;
        _userRepository = userRepository;
    }

    public async Task<Result<LoginResponse>> HandleAsync(RefreshTokenCommand command)
    {
        var principal = _jwtProvider.GetPrincipalFromToken(command.AccessToken);

        if (principal is null)
            return new Result<LoginResponse>()
            {
                Error = "Token inválido.",
                Success = false,
                StatusCode = HttpStatusCode.Unauthorized
            };

        var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrWhiteSpace(userId))
            return new Result<LoginResponse>()
            {
                Error = "Token inválido.",
                Success = false,
                StatusCode = HttpStatusCode.Unauthorized
            };

        var user = await _userRepository.GetByIdAsync(Guid.Parse(userId));

        if (user is null)
            return new Result<LoginResponse>()
            {
                Error = "Usuário não encontrado.",
                Success = false,
                StatusCode = HttpStatusCode.Unauthorized
            };

        var token = _jwtProvider.GenerateToken(user);

        return new Result<LoginResponse>()
        {
            Success = true,
            Data = new LoginResponse
            {
                AccessToken = token,
                ExpiresAt = DateTime.UtcNow.AddHours(1)
            }
        };
    }
}
