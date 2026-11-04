using System.Net;
using System.Security.Claims;
using Techfin.Case.Application.DTOs.Responses;
using Techfin.Case.Application.Interfaces;
using Techfin.Case.Application.UseCases.RefreshToken;
using Techfin.Case.Application.Validators;
using Techfin.Case.Domain.Repositories;
using Techfin.Shared;

namespace Techfin.Case.Application.UseCases.Authentication;
public sealed class AuthUserHandler(IUserRepository repository, IPasswordVerify passwordHasher, IJwtProvider jwtProvider)
{
    private readonly IUserRepository _repository = repository;
    private readonly IPasswordVerify _passwordVerify = passwordHasher;
    private readonly IJwtProvider _jwtProvider = jwtProvider;

    public async Task<Result<LoginResponse>> HandleAsync(AuthUserCommand command)
    {
        var validator = new LoginRequestValidator();
        var validationResult = await validator!.ValidateAsync(command);

        if (!validationResult.IsValid)
        {
            return new Result<LoginResponse>
            {
                Success = false,
                Message = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)),
                StatusCode = HttpStatusCode.BadRequest
            };
        }

        var user = await _repository.GetByEmailAsync(command.Email);

        if (user is null)
        {
            return new Result<LoginResponse>()
            {
                Message = "Email Techfin inválido ou não encontrado.",
                Success = false,
                StatusCode = HttpStatusCode.Unauthorized
            };
        }

        var validPassword = _passwordVerify.Verify(command.Password, user.Password);

        if (!validPassword)
        {
            return new Result<LoginResponse>()
            {
                Message = "Senha não confere.",
                Success = false,
                StatusCode = HttpStatusCode.Unauthorized
            };
        }

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

    public async Task<Result<LoginResponse>> RefreshTokenAsync(RefreshTokenCommand command)
    {
        var principal = _jwtProvider.GetPrincipalFromToken(command.AccessToken);

        if (principal is null)
            return new Result<LoginResponse>()
            {
                Message = "Token inválido.",
                Success = false,
                StatusCode = HttpStatusCode.Unauthorized
            };

        var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrWhiteSpace(userId))
            return new Result<LoginResponse>()
            {
                Message = "Token inválido.",
                Success = false,
                StatusCode = HttpStatusCode.Unauthorized
            };

        var user = await _repository.GetByIdAsync(Guid.Parse(userId));

        if (user is null)
            return new Result<LoginResponse>()
            {
                Message = "Usuário não encontrado.",
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
                ExpiresAt = DateTime.Now.AddHours(1)
            }
        };
    }
}