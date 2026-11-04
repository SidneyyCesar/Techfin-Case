using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using Techfin.Contracts;
using Techfin.Customer.Domain.Repositories;
using Techfin.Shared;

namespace Techfin.Customer.Infra.Messaging;

public sealed class TransactionCreatedConsumer : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly RabbitMqSettings _settings;

    public TransactionCreatedConsumer(IServiceProvider serviceProvider, IOptions<RabbitMqSettings> options)
    {
        _serviceProvider = serviceProvider;
        _settings = options.Value;
    }
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var scope = _serviceProvider.CreateScope();

        var factory = new ConnectionFactory { HostName = _settings.HostName };

        var connection = factory.CreateConnection();

        var channel = connection.CreateModel();

        channel.ExchangeDeclare(_settings.Exchange, ExchangeType.Fanout, durable: true);

        channel.QueueDeclare(_settings.Queue, durable: true, exclusive: false, autoDelete: false);

        channel.QueueBind(_settings.Queue, _settings.Exchange, string.Empty);

        var consumer = new EventingBasicConsumer(channel);

        consumer.Received += async (_, ea) =>
        {
            var json = Encoding.UTF8.GetString(ea.Body.ToArray());

            var evt = JsonSerializer.Deserialize<TransactionCreatedEvent>(json);

            if (evt is null)
                return;

            await ProcessEvent(evt);
        };

        channel.BasicConsume(_settings.Queue, true, consumer);

        return Task.CompletedTask;
    }

    private async Task ProcessEvent(TransactionCreatedEvent evt)
    {
        using var scope = _serviceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<ICustomerRepository>();

        var customer = await repository.GetByIdAsync(evt.CustomerId);

        if (customer is null)
            return;

        customer.DebitLimit(evt.Amount);

        await repository.UpdateAsync(customer);
    }
}
