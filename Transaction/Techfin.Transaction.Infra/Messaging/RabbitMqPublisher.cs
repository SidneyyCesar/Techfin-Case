using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;
using Techfin.Shared;

namespace Techfin.Transaction.Infra.Messaging;

public sealed class RabbitMqPublisher : IMessagePublisher
{
    private readonly RabbitMqSettings _settings;

    public RabbitMqPublisher(IOptions<RabbitMqSettings> options)
    {
        _settings = options.Value;
    }
    public Task PublishAsync<T>(T message)
    {
        var factory = new ConnectionFactory { HostName = _settings.HostName };

        using var connection = factory.CreateConnection();

        using var channel = connection.CreateModel();

        channel.ExchangeDeclare(exchange: _settings.Exchange, type: ExchangeType.Fanout, durable: true);

        var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));

        channel.BasicPublish(exchange: _settings.Exchange, routingKey: "", body: body);

        channel.QueueDeclare(queue: _settings.Queue, durable: true, exclusive: false, autoDelete: false);

        return Task.CompletedTask;
    }
    public Task<bool> IsAvailableAsync()
    {
        try
        {
            var factory = new ConnectionFactory { HostName = _settings.HostName };

            using var connection = factory.CreateConnection();

            return Task.FromResult(true);
        }
        catch
        {
            return Task.FromResult(false);
        }
    }
}
