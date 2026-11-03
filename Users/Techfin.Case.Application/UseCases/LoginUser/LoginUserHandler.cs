using System.Net;
using Techfin.Case.Application.DTOs.Responses;
using Techfin.Case.Application.Interfaces;
using Techfin.Case.Application.Validators;
using Techfin.Case.Domain.Repositories;
using Techfin.Shared;

namespace Techfin.Case.Application.UseCases.LoginUser;

public sealed class LoginUserHandler
{
    private readonly IUserRepository _repository;
    private readonly IPasswordVerify _passwordVerify;
    private readonly IJwtProvider _jwtProvider;

    public LoginUserHandler(IUserRepository repository, IPasswordVerify passwordHasher, IJwtProvider jwtProvider)
    {
        _repository = repository;
        _passwordVerify = passwordHasher;
        _jwtProvider = jwtProvider;
    }

    public async Task<Result<LoginResponse>> HandleAsync(LoginUserCommand command)
    {
        // Validação dos dados do request usando FluentValidation
        var validator = new LoginRequestValidator();
        var validationResult = await validator!.ValidateAsync(command);

        if (!validationResult.IsValid)
        {
            return new Result<LoginResponse>
            {
                Success = false,
                Error = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)),
                StatusCode = HttpStatusCode.BadRequest
            };
        }

        var user = await _repository.GetByEmailAsync(command.Email);

        if (user is null)
        {
            return new Result<LoginResponse>()
            {
                Error = "Email Techfin inválido ou não encontrado.",
                Success = false,
                StatusCode = HttpStatusCode.Unauthorized
            };
        }

        var validPassword = _passwordVerify.Verify(command.Password, user.Password);

        if (!validPassword)
        {
            return new Result<LoginResponse>()
            {
                Error = "Senha não confere.",
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
}