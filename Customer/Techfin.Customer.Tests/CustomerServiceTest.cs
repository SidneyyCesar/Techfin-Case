using FluentAssertions;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using System.Net;
using Techfin.Customer.Application.DTOs.Requests;
using Techfin.Customer.Application.Services;
using Techfin.Customer.Domain.Repositories;

namespace Techfin.Customer.Tests;
public class CustomerServiceTest
{
    private readonly Mock<ICustomerRepository> _repositoryMock;
    private readonly IMemoryCache _cache;
    private readonly CustomerService _service;

    public CustomerServiceTest()
    {
        _repositoryMock = new Mock<ICustomerRepository>();
        _cache = new MemoryCache(new MemoryCacheOptions());
        _service = new CustomerService(_repositoryMock.Object, _cache);
    }

    [Fact]
    public async Task GetByIdAsync_DeveRetornarCliente()
    {
        var id = Guid.NewGuid();

        var customer = new Domain.Customer(id, "Sidney", "12345678900", 5000);

        _repositoryMock.Setup(x => x.GetByIdAsync(id)).ReturnsAsync(customer);

        var result = await _service.GetByIdAsync(id);

        result.Success.Should().BeTrue();
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        result.Data.Should().NotBeNull();
        result.Data!.Id.Should().Be(id);
        result.Data.Nome.Should().Be("Sidney");
        result.Data.CPF.Should().Be("12345678900");

        _repositoryMock.Verify(x => x.GetByIdAsync(id), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_DeveRetornarNaoEncontrado()
    {
        var id = Guid.NewGuid();

        _repositoryMock
            .Setup(x => x.GetByIdAsync(id))
            .ReturnsAsync((Domain.Customer?)null);

        var result = await _service.GetByIdAsync(id);

        result.Success.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
        result.Message.Should().Be(
            "Não foi encontrado um cliente com o ID informado"
        );

        result.Data.Should().BeNull();
    }

    [Fact]
    public async Task ListAsync_DeveRetornarLista()
    {
        var customers = new List<Domain.Customer>
        {
            new (Guid.NewGuid(), "Sidney", "12345678900", 5000),
            new (Guid.NewGuid(), "Maria", "98765432100", 8000)
        };

        _repositoryMock.Setup(x => x.GetListAsync()).ReturnsAsync(customers);

        var result = await _service.ListAsync();

        result.Success.Should().BeTrue();
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        result.Data.Should().HaveCount(2);
        result.Data![0].Nome.Should().Be("Sidney");
        result.Data[1].Nome.Should().Be("Maria");

        _repositoryMock.Verify(x => x.GetListAsync(), Times.Once);
    }

    [Fact]
    public async Task ListAsync_DeveUserCache_SegundaChamada()
    {
        var customers = new List<Domain.Customer>
        {
            new (Guid.NewGuid(), "Sidney", "12345678900", 5000)
        };

        _repositoryMock.Setup(x => x.GetListAsync()).ReturnsAsync(customers);

        var firstResult = await _service.ListAsync();
        var secondResult = await _service.ListAsync();

        firstResult.Data.Should().HaveCount(1);
        secondResult.Data.Should().HaveCount(1);

        _repositoryMock.Verify(x => x.GetListAsync(), Times.Once);
    }

    [Fact]
    public async Task IncludeAsync_DeveIncluir_QuandoDadosValidos()
    {
        var request = new CustomerInsertRequest
        {
            Nome = "Sidney",
            CPF = "12345678900",
            ValorLimite = 5000
        };

        _repositoryMock.Setup(x => x.GetByCpfAsync(request.CPF)).ReturnsAsync((Domain.Customer?)null);

        _repositoryMock.Setup(x => x.AddAsync(It.IsAny<Domain.Customer>())).Returns(Task.CompletedTask);

        var result = await _service.IncludeAsync(request);

        result.Success.Should().BeTrue();
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        result.Message.Should().Be("Cliente cadastrado com sucesso");
        result.Data.Should().NotBeNull();
        result.Data!.Nome.Should().Be(request.Nome);
        result.Data.CPF.Should().Be(request.CPF);

        _repositoryMock.Verify(
            x => x.GetByCpfAsync(request.CPF),
            Times.Once
        );

        _repositoryMock.Verify(
            x => x.AddAsync(It.Is<Domain.Customer>(c =>
                c.Name == request.Nome &&
                c.CPF == request.CPF
            )),
            Times.Once
        );
    }

    [Fact]
    public async Task IncludeAsync_DeveRetornarConflito_ClienteMesmoCPF()
    {
        var request = new CustomerInsertRequest
        {
            Nome = "Sidney",
            CPF = "12345678900",
            ValorLimite = 5000
        };

        var customer = new Domain.Customer(
            Guid.NewGuid(),
            "Outro cliente",
            request.CPF,
            3000
        );

        _repositoryMock.Setup(x => x.GetByCpfAsync(request.CPF)).ReturnsAsync(customer);

        var result = await _service.IncludeAsync(request);

        result.Success.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.Conflict);
        result.Message.Should().Be(
            "Já existe um cliente cadastrado com este CPF"
        );

        _repositoryMock.Verify(
            x => x.AddAsync(It.IsAny<Domain.Customer>()),
            Times.Never
        );
    }

    [Fact]
    public async Task IncludeAsync_DeveRetornarFalha_DadosInvalidos()
    {
        var request = new CustomerInsertRequest
        {
            Nome = "",
            CPF = "",
            ValorLimite = 0
        };

        var result = await _service.IncludeAsync(request);

        result.Success.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        result.Message.Should().NotBeNullOrEmpty();

        _repositoryMock.Verify(
            x => x.GetByCpfAsync(It.IsAny<string>()),
            Times.Never
        );

        _repositoryMock.Verify(
            x => x.AddAsync(It.IsAny<Domain.Customer>()),
            Times.Never
        );
    }

    [Fact]
    public async Task UpdateAsync_DeveAtualizarCliente_SeExistir()
    {
        var id = Guid.NewGuid();

        var request = new CustomerUpdateRequest(id.ToString(), "Sidney Atualizado", "12345678900", 10000);

        var existingCustomer = new Domain.Customer(
            id,
            "Sidney",
            request.CPF,
            5000
        );

        _repositoryMock.Setup(x => x.GetByIdAsync(id)).ReturnsAsync(existingCustomer);

        var result = await _service.UpdateAsync(request);

        result.Success.Should().BeTrue();
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        result.Data!.Id.Should().Be(id);
        result.Data.Nome.Should().Be(request.Nome);
        result.Data.ValorLimite.Should().Be(request.ValorLimite);

        _repositoryMock.Verify(
            x => x.UpdateAsync(It.Is<Domain.Customer>(c =>
                c.Id == id &&
                c.Name == request.Nome &&
                c.CPF == request.CPF
            ), 0),
            Times.Once
        );
    }

    [Fact]
    public async Task UpdateAsync_DeveBloquearAtualizacao_ClienteNaoExiste()
    {
        var id = Guid.NewGuid();

        var request = new CustomerUpdateRequest(id.ToString(), "Sidney", "12345678900", 5000);

        _repositoryMock.Setup(x => x.GetByIdAsync(id)).ReturnsAsync((Domain.Customer?)null);

        var result = await _service.UpdateAsync(request);

        result.Success.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
        result.Message.Should().Be("Cliente não encontrado");

        _repositoryMock.Verify(
            x => x.UpdateAsync(It.IsAny<Domain.Customer>(), 0),
            Times.Never
        );
    }

    [Fact]
    public async Task UpdateAsync_DeveRetornarFalha_SolicitacaoInvalida()
    {
        var request = new CustomerUpdateRequest("", "", "", 0);

        var result = await _service.UpdateAsync(request);

        result.Success.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        _repositoryMock.Verify(
            x => x.GetByIdAsync(It.IsAny<Guid>()),
            Times.Never
        );
    }

    [Fact]
    public async Task DeleteAsync_DeveDeletarCliente()
    {
        var id = Guid.NewGuid();

        var customer = new Domain.Customer(
            id,
            "Sidney",
            "12345678900",
            5000
        );

        _repositoryMock.Setup(x => x.GetByIdAsync(id)).ReturnsAsync(customer);

        _repositoryMock
            .Setup(x => x.DeleteAsync(id.ToString().ToUpper()))
            .Returns(Task.CompletedTask);

        var result = await _service.DeleteAsync(id.ToString());

        result.Success.Should().BeTrue();
        result.Data.Should().BeTrue();
        result.Message.Should().Be("Cliente deletado com sucesso");

        _repositoryMock.Verify(
            x => x.DeleteAsync(id.ToString().ToUpper()),
            Times.Once
        );
    }

    [Fact]
    public async Task DeleteAsync_DeveRetornarNaoEncontrado()
    {
        var id = Guid.NewGuid();

        _repositoryMock
            .Setup(x => x.GetByIdAsync(id))
            .ReturnsAsync((Domain.Customer?)null);

        var result = await _service.DeleteAsync(id.ToString());

        result.Success.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
        result.Message.Should().Be("Cliente não encontrado");

        _repositoryMock.Verify(
            x => x.DeleteAsync(It.IsAny<string>()),
            Times.Never
        );
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public async Task DeleteAsync_DeveRetornar_ClienteInvalido(string? id)
    {
        var result = await _service.DeleteAsync(id!);

        result.Success.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.NoContent);
        result.Message.Should().Be("Não foi informado o ID do cliente");

        _repositoryMock.Verify(
            x => x.GetByIdAsync(It.IsAny<Guid>()),
            Times.Never
        );
    }
}