using Moq;
using System.Net;
using System.Security.Claims;
using Techfin.Case.Application.Interfaces;
using Techfin.Case.Application.UseCases.Authentication;
using Techfin.Case.Application.UseCases.RefreshToken;
using Techfin.Case.Domain.Entities;
using Techfin.Case.Domain.Repositories;

namespace Techfin.Users.Tests;

public class AuthUserHandlerTests
{
    private readonly Mock<IUserRepository> _repositoryMock;
    private readonly Mock<IPasswordVerify> _passwordVerifyMock;
    private readonly Mock<IJwtProvider> _jwtProviderMock;

    private readonly AuthUserHandler _handler;

    private readonly Guid _userId = Guid.NewGuid();

    public AuthUserHandlerTests()
    {
        _repositoryMock = new Mock<IUserRepository>();
        _passwordVerifyMock = new Mock<IPasswordVerify>();
        _jwtProviderMock = new Mock<IJwtProvider>();

        _handler = new AuthUserHandler(_repositoryMock.Object, _passwordVerifyMock.Object, _jwtProviderMock.Object);
    }

    private static User CriarUsuario() => new(Guid.NewGuid(), "Sidney", "sidney@techfin.com", "senha-hash", DateTime.Now);

    private static ClaimsPrincipal CriarPrincipal(Guid? userId = null)
    {
        var claims = new List<Claim>();

        if (userId.HasValue)
        {
            claims.Add(new Claim(
                ClaimTypes.NameIdentifier,
                userId.Value.ToString()
            ));
        }

        var identity = new ClaimsIdentity(claims, "Bearer");

        return new ClaimsPrincipal(identity);
    }


    [Fact]
    public async Task DeveRetornarBadRequest_QuandoDadosDoLoginForemInvalidos()
    {
        var command = new AuthUserCommand("", "");

        var result = await _handler.HandleAsync(command);

        Assert.False(result.Success);
        Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
        Assert.False(string.IsNullOrEmpty(result.Message));

        _repositoryMock.Verify(
            x => x.GetByEmailAsync(It.IsAny<string>()),
            Times.Never
        );

        _passwordVerifyMock.Verify(
            x => x.Verify(It.IsAny<string>(), It.IsAny<string>()),
            Times.Never
        );

        _jwtProviderMock.Verify(
            x => x.GenerateToken(It.IsAny<User>()),
            Times.Never
        );
    }

    [Fact]
    public async Task DeveRetornarUnauthorized_QuandoEmailNaoForEncontrado()
    {
        var command = new AuthUserCommand("inexistente@techfin.com", "123456");

        _repositoryMock
            .Setup(x => x.GetByEmailAsync(command.Email))
            .ReturnsAsync((User?)null);

        var result = await _handler.HandleAsync(command);

        Assert.False(result.Success);
        Assert.Equal(HttpStatusCode.Unauthorized, result.StatusCode);
        Assert.Equal("Email Techfin inválido ou não encontrado.", result.Message);

        _passwordVerifyMock.Verify(x => x.Verify(It.IsAny<string>(), It.IsAny<string>()), Times.Never);

        _jwtProviderMock.Verify(x => x.GenerateToken(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task DeveRetornarUnauthorized_QuandoSenhaNaoConferir()
    {
        var command = new AuthUserCommand("sidney@techfin.com", "senha-incorreta");

        var user = CriarUsuario();

        _repositoryMock
            .Setup(x => x.GetByEmailAsync(command.Email))
                .ReturnsAsync(user);

        _passwordVerifyMock
            .Setup(x => x.Verify(command.Password, user.Password))
                .Returns(false);

        var result = await _handler.HandleAsync(command);

        Assert.False(result.Success);
        Assert.Equal(HttpStatusCode.Unauthorized, result.StatusCode);
        Assert.Equal("Senha não confere.", result.Message);

        _jwtProviderMock.Verify(
            x => x.GenerateToken(It.IsAny<User>()),
            Times.Never
        );
    }

    [Fact]
    public async Task DeveGerarNovoToken_QuandoTokenForValido()
    {
        var command = new RefreshTokenCommand("token-atual");

        var user = CriarUsuario();
        var principal = CriarPrincipal(_userId);

        _jwtProviderMock
            .Setup(x => x.GetPrincipalFromToken(command.AccessToken))
            .Returns(principal);

        _repositoryMock
            .Setup(x => x.GetByIdAsync(_userId))
            .ReturnsAsync(user);

        _jwtProviderMock
            .Setup(x => x.GenerateToken(user))
            .Returns("novo-jwt-token");

        var result = await _handler.RefreshTokenAsync(command);

        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.Equal("novo-jwt-token", result.Data.AccessToken);
        Assert.True(result.Data.ExpiresAt > DateTime.Now);

        _jwtProviderMock.Verify(x => x.GetPrincipalFromToken(command.AccessToken), Times.Once);

        _repositoryMock.Verify(x => x.GetByIdAsync(_userId), Times.Once);

        _jwtProviderMock.Verify(x => x.GenerateToken(user), Times.Once);
    }

    [Fact]
    public async Task DeveRetornarUnauthorized_QuandoTokenForInvalido()
    {
        var command = new RefreshTokenCommand("token-invalido");

        _jwtProviderMock
            .Setup(x => x.GetPrincipalFromToken(command.AccessToken))
            .Returns((ClaimsPrincipal?)null);

        var result = await _handler.RefreshTokenAsync(command);

        Assert.False(result.Success);
        Assert.Equal(HttpStatusCode.Unauthorized, result.StatusCode);
        Assert.Equal("Token inválido.", result.Message);

        _repositoryMock.Verify(x => x.GetByIdAsync(It.IsAny<Guid>()), Times.Never);

        _jwtProviderMock.Verify(x => x.GenerateToken(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task DeveRetornarUnauthorized_QuandoTokenNaoPossuirIdDoUsuario()
    {
        var command = new RefreshTokenCommand("token-sem-id");

        var principal = CriarPrincipal();

        _jwtProviderMock
            .Setup(x => x.GetPrincipalFromToken(command.AccessToken))
            .Returns(principal);

        var result = await _handler.RefreshTokenAsync(command);

        Assert.False(result.Success);
        Assert.Equal(HttpStatusCode.Unauthorized, result.StatusCode);
        Assert.Equal("Token inválido.", result.Message);

        _repositoryMock.Verify(x => x.GetByIdAsync(It.IsAny<Guid>()), Times.Never);

        _jwtProviderMock.Verify(x => x.GenerateToken(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task DeveRetornarUnauthorized_QuandoUsuarioNaoForEncontrado()
    {
        var command = new RefreshTokenCommand("token-usuario-inexistente");

        var principal = CriarPrincipal(_userId);

        _jwtProviderMock.Setup(x => x.GetPrincipalFromToken(command.AccessToken)).Returns(principal);

        _repositoryMock.Setup(x => x.GetByIdAsync(_userId)).ReturnsAsync((User?)null);

        var result = await _handler.RefreshTokenAsync(command);

        Assert.False(result.Success);
        Assert.Equal(HttpStatusCode.Unauthorized, result.StatusCode);
        Assert.Equal("Usuário não encontrado.", result.Message);

        _jwtProviderMock.Verify(x => x.GenerateToken(It.IsAny<User>()), Times.Never);
    }
}