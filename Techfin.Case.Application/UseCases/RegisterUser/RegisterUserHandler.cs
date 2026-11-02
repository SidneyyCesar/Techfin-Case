using Techfin.Case.Application.Interfaces;
using Techfin.Case.Application.Validators;
using Techfin.Case.Domain.Entities;
using Techfin.Case.Domain.Repositories;

namespace Techfin.Case.Application.UseCases.RegisterUser;
public sealed class RegisterUserHandler
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordVerify _passwordVerify;
    private Notifications _notification;

    public RegisterUserHandler(IUserRepository userRepository, IPasswordVerify passwordVerify)
    {
        _userRepository = userRepository;
        _passwordVerify = passwordVerify;
        _notification = new Notifications();
    }

    public async Task<(Guid, Notifications)> HandleAsync(RegisterUserCommand command)
    {
        // Validação dos dados do request usando FluentValidation
        var validator = new RegisterUserValidator();
        var validationResult = await validator!.ValidateAsync(command);

        if (!validationResult.IsValid)
        {
            _notification = new Notifications(string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)));
            return (Guid.Empty, _notification);
        }

        // Validação para evitar duplicidade de usuários com o mesmo e-mail
        bool exists = await _userRepository.ExistsByEmailAsync(command.Email);

        if (exists)
        {
            _notification = new Notifications("Usuário já existente");
            return (Guid.Empty, _notification);
        }

        // Criação do usuário com a senha criptografada
        string passwordHash = _passwordVerify.Hash(command.Password);

        User user = new(command.Name, command.Email, passwordHash);

        await _userRepository.AddAsync(user);

        return (user.Id, _notification);
    }
    public Notifications GetNotifications()
    {
        return _notification;
    }
}
