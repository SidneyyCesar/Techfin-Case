using FluentAssertions;
using Moq;
using Moq.Protected;
using System.Net;
using System.Text;
using Techfin.Contracts;
using Techfin.Transaction.Application.Messaging;
using Techfin.Transaction.Domain.Repositories;
using Techfin.Transaction.Infra;

namespace Techfin.Transaction.Tests
{
    public class TransactionHandlerTests
    {
        private readonly Mock<ITransactionRepository> _repositoryMock;
        private readonly Mock<IMessagePublisher> _publisherMock;
        private readonly Mock<HttpMessageHandler> _httpHandlerMock;

        private readonly HttpClient _httpClient;
        private readonly CreateTransactionHandler _handler;

        private readonly Guid _customerId = Guid.NewGuid();

        public TransactionHandlerTests()
        {
            _repositoryMock = new Mock<ITransactionRepository>();
            _publisherMock = new Mock<IMessagePublisher>();
            _httpHandlerMock = new Mock<HttpMessageHandler>();

            _httpClient = new HttpClient(_httpHandlerMock.Object);

            _handler = new CreateTransactionHandler(
                _repositoryMock.Object,
                _publisherMock.Object,
                _httpClient
            );
        }

        private CreateTransactionCommand CriarComando(decimal amount = 100, string transactionKey = "TRANSACAO-001")
        {
            return new CreateTransactionCommand(_customerId, amount, transactionKey, "token");
        }

        private void ConfigurarClienteApi(HttpStatusCode statusCode = HttpStatusCode.OK, decimal limite = 1000)
        {
            var json = statusCode == HttpStatusCode.OK
                ? $$"""
                {
                    "data": {
                        "Id": "{{_customerId}}",
                        "Nome": "Sidney",
                        "ValorLimite": {{limite}}
                    }
                }
                """
                : "{}";

            var response = new HttpResponseMessage(statusCode)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };

            _httpHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                ).ReturnsAsync(response);
        }

        private void ConfigurarServicosDisponiveis()
        {
            _publisherMock.Setup(x => x.IsAvailableAsync()).ReturnsAsync(true);

            _repositoryMock.Setup(x => x.IsAvailableAsync()).ReturnsAsync(true);

            _repositoryMock.Setup(x => x.GetByRequestIdAsync(It.IsAny<string>())).ReturnsAsync((Domain.Transaction?)null);
        }

        [Fact]
        public async Task DeveCriarTransacao_QuandoDadosForemValidos()
        {
            var command = CriarComando();

            ConfigurarClienteApi();
            ConfigurarServicosDisponiveis();

            _repositoryMock.Setup(x => x.AddAsync(It.IsAny<Domain.Transaction>())).Returns(Task.CompletedTask);

            _publisherMock.Setup(x => x.PublishAsync(It.IsAny<TransactionCreatedEvent>())).Returns(Task.CompletedTask);

            var result = await _handler.HandleAsync(command);

            result.Success.Should().BeTrue();
            result.StatusCode.Should().Be(HttpStatusCode.OK);
            result.Data.Should().NotBeNull();
            result.Data!.ChaveTransacao.Should().Be(command.TransactionKey);
            result.Data.Status.Should().Be("Aprovado");

            _repositoryMock.Verify(x => x.AddAsync(It.IsAny<Domain.Transaction>()), Times.Once);

            _publisherMock.Verify(x => x.PublishAsync(It.IsAny<TransactionCreatedEvent>()), Times.Once);
        }

        [Fact]
        public async Task DeveNegarTransacao_QuandoClienteNaoForEncontrado()
        {
            var command = CriarComando();

            ConfigurarClienteApi(HttpStatusCode.NotFound);

            var result = await _handler.HandleAsync(command);

            // Assert
            result.Success.Should().BeFalse();
            result.Message.Should().Be("Não foi possível localizar o cliente desta transação");

            result.Data!.Status.Should().Be("Negado");
            result.Data.ChaveTransacao.Should().Be(command.TransactionKey);

            _repositoryMock.Verify(
                x => x.AddAsync(It.IsAny<Domain.Transaction>()),
                Times.Never);

            _publisherMock.Verify(
                x => x.PublishAsync(
                    It.IsAny<TransactionCreatedEvent>()),
                Times.Never);
        }

        [Fact]
        public async Task DeveNegarTransacao_QuandoValorForSuperiorAoLimiteDoCliente()
        {
            var command = CriarComando(amount: 1500);

            ConfigurarClienteApi(limite: 1000);
            ConfigurarServicosDisponiveis();

            var result = await _handler.HandleAsync(command);

            result.Success.Should().BeFalse();
            result.Data!.Status.Should().Be("Negado");

            result.Message.Should().Contain("O valor da transação é superior ao limite disponível");

            _repositoryMock.Verify(x => x.AddAsync(It.IsAny<Domain.Transaction>()), Times.Never);

            _publisherMock.Verify(x => x.PublishAsync(It.IsAny<TransactionCreatedEvent>()), Times.Never);
        }

        [Fact]
        public async Task DeveNegarTransacao_QuandoRabbitMqEstiverIndisponivel()
        {
            var command = CriarComando();

            ConfigurarClienteApi();
            ConfigurarServicosDisponiveis();

            _publisherMock.Setup(x => x.IsAvailableAsync()).ReturnsAsync(false);

            var result = await _handler.HandleAsync(command);

            result.Success.Should().BeFalse();
            result.Data!.Status.Should().Be("Negado");

            result.Message.Should().Be("Serviço indisponível no momento, tente novamente mais tarde.");

            _repositoryMock.Verify(x => x.AddAsync(It.IsAny<Domain.Transaction>()), Times.Never);

            _publisherMock.Verify(x => x.PublishAsync(It.IsAny<TransactionCreatedEvent>()), Times.Never);
        }

        [Fact]
        public async Task DeveNegarTransacao_QuandoBancoEstiverIndisponivel()
        {
            var command = CriarComando();

            ConfigurarClienteApi();
            ConfigurarServicosDisponiveis();

            _repositoryMock.Setup(x => x.IsAvailableAsync()).ReturnsAsync(false);

            var result = await _handler.HandleAsync(command);

            result.Success.Should().BeFalse();
            result.Data!.Status.Should().Be("Negado");

            result.Message.Should().Be("Serviço indisponível no momento, tente novamente mais tarde.");

            _repositoryMock.Verify(
                x => x.AddAsync(It.IsAny<Domain.Transaction>()),
                Times.Never);

            _publisherMock.Verify(
                x => x.PublishAsync(
                    It.IsAny<TransactionCreatedEvent>()),
                Times.Never);
        }

        [Fact]
        public async Task DeveNegarTransacao_QuandoChaveJaFoiUtilizada()
        {
            var command = CriarComando();

            ConfigurarClienteApi();
            ConfigurarServicosDisponiveis();

            var existingTransaction = new Domain.Transaction(
                _customerId,
                100,
                command.TransactionKey
            );

            _repositoryMock
                .Setup(x => x.GetByRequestIdAsync(command.TransactionKey))
                .ReturnsAsync(existingTransaction);

            var result = await _handler.HandleAsync(command);

            result.Success.Should().BeFalse();
            result.Data!.Status.Should().Be("Negado");

            result.Message.Should().Contain("já foi executada anteriormente");

            _repositoryMock.Verify(
                x => x.AddAsync(It.IsAny<Domain.Transaction>()),
                Times.Never);

            _publisherMock.Verify(
                x => x.PublishAsync(
                    It.IsAny<TransactionCreatedEvent>()),
                Times.Never);
        }

        [Fact]
        public async Task DeveNegarTransacao_QuandoValorForNegativo()
        {
            var command = CriarComando(amount: -100);

            ConfigurarClienteApi();
            ConfigurarServicosDisponiveis();

            var result = await _handler.HandleAsync(command);

            result.Success.Should().BeFalse();
            result.Data!.Status.Should().Be("Negado");

            result.Message.Should().Be("O valor da transação não pode ser negativo.");

            _repositoryMock.Verify(x => x.AddAsync(It.IsAny<Domain.Transaction>()), Times.Never);
        }

        [Fact]
        public async Task DeveNegarTransacao_QuandoChaveNaoForInformada()
        {
            var command = CriarComando(transactionKey: "");

            ConfigurarClienteApi();
            ConfigurarServicosDisponiveis();

            var result = await _handler.HandleAsync(command);

            result.Success.Should().BeFalse();
            result.Data!.Status.Should().Be("Negado");

            result.Message.Should().Be("Identificador da transação não informado.");

            _repositoryMock.Verify(x => x.AddAsync(It.IsAny<Domain.Transaction>()), Times.Never);
        }
    }
}
