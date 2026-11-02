using Techfin.Case.Application.DTOs.Responses;
using Techfin.Case.Application.Exceptions;
using Techfin.Case.Application.Interfaces;
using Techfin.Case.Domain.Repositories;

namespace Techfin.Case.Application.UseCases.LoginUser;
public class LoginUserHandler
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordVerify _passwordHasher;
    private readonly IJwtProvider _jwtProvider;

    public LoginUserHandler(IUserRepository userRepository, IPasswordVerify passwordHasher, IJwtProvider jwtProvider)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _jwtProvider = jwtProvider;
    }
    public async Task<LoginResponse> HandleAsync(
    LoginUserCommand command)
    {
        Domain.Entities.User? user =
            await _userRepository.GetByEmailAsync(command.Email);

        if (user is null)
        {
            throw new UnauthorizedException("Credenciais Techfin inválidas.");
        }

        bool passwordValid = _passwordHasher.Verify(command.Password, user.Password);

        if (!passwordValid)
        {
            throw new UnauthorizedException("Invalid credentials");
        }

        string token = _jwtProvider.GenerateToken(user);

        return new LoginResponse
        {
            AccessToken = token,
            ExpiresAt = DateTime.UtcNow.AddHours(1)
        };
    }
}