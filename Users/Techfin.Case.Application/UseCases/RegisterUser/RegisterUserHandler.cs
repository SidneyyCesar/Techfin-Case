using System.Net;
using Techfin.Case.Application.Interfaces;
using Techfin.Case.Application.Validators;
using Techfin.Case.Domain.Entities;
using Techfin.Case.Domain.Repositories;

namespace Techfin.Case.Application.UseCases.RegisterUser;
public sealed class RegisterUserHandler
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordVerify _passwordVerify;

    public RegisterUserHandler(IUserRepository userRepository, IPasswordVerify passwordVerify)
    {
        _userRepository = userRepository;
        _passwordVerify = passwordVerify;
    }

    public async Task<Result<Guid>> HandleAsync(RegisterUserCommand command)
    {
        // Validação dos dados do request usando FluentValidation
        var validator = new RegisterUserValidator();
        var validationResult = await validator!.ValidateAsync(command);

        if (!validationResult.IsValid)
        {
            return new Result<Guid>
            {
                Success = false,
                Error = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)),
                Data = Guid.Empty,
                StatusCode = HttpStatusCode.BadRequest
            };
        }

        // Validação para evitar duplicidade de usuários com o mesmo e-mail
        bool exists = await _userRepository.ExistsByEmailAsync(command.Email);

        if (exists)
        {
            return new Result<Guid>
            {
                Success = false,
                Error = "Usuário já existente",
                Data = Guid.Empty,
                StatusCode = HttpStatusCode.Conflict
            };
        }

        // Criação do usuário com a senha criptografada
        string passwordCripto = _passwordVerify.Hash(command.Password);

        User user = new(command.Name, command.Email, passwordCripto);

        await _userRepository.AddAsync(user);

        return new Result<Guid>
        {
            Success = true,
            Error = "Usuário cadastrado com sucesso",
            Data = user.Id
        };
    }
}
