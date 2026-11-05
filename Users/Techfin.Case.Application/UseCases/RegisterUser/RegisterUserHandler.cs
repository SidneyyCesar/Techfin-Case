using System.Net;
using Techfin.Case.Application.Interfaces;
using Techfin.Case.Application.Validators;
using Techfin.Case.Domain.Entities;
using Techfin.Case.Domain.Repositories;
using Techfin.Shared;

namespace Techfin.Case.Application.UseCases.RegisterUser;

/// <summary>
/// Classe responsável pela criação de novos usuários
/// </summary>
/// <param name="userRepository">repositório de usuários recebido via DI</param>
/// <param name="passwordVerify">classe de criptografia de senha</param>
public sealed class RegisterUserHandler(IUserRepository userRepository, IPasswordVerify passwordVerify)
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IPasswordVerify _passwordVerify = passwordVerify;

    public async Task<Result<Guid?>> HandleAsync(RegisterUserCommand command)
    {
        // Validação dos dados do request usando FluentValidation
        var validator = new RegisterUserValidator();
        var validationResult = await validator!.ValidateAsync(command);

        if (!validationResult.IsValid)
        {
            return new Result<Guid?>
            {
                Success = false,
                Message = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)),
                StatusCode = HttpStatusCode.BadRequest
            };
        }

        // Validação para evitar duplicidade de usuários com o mesmo e-mail
        bool exists = await _userRepository.ExistsByEmailAsync(command.Email);

        if (exists)
        {
            return new Result<Guid?>
            {
                Success = false,
                Message = "Usuário já existente",
                StatusCode = HttpStatusCode.Conflict
            };
        }

        // Criação do usuário com a senha criptografada
        string passwordCripto = _passwordVerify.Hash(command.Password);

        var user = new User(Guid.NewGuid(),
                            command.Name,
                            command.Email,
                            passwordCripto,
                            DateTime.Now);

        await _userRepository.AddAsync(user);

        return new Result<Guid?>
        {
            Success = true,
            StatusCode = HttpStatusCode.OK,
            Message = "Usuário cadastrado com sucesso",
            Data = user.Id
        };
    }
}
