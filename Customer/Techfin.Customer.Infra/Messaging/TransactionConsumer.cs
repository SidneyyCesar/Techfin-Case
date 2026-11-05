using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using Techfin.Contracts;
using Techfin.Customer.Domain.Entities;
using Techfin.Customer.Domain.Repositories;
using Techfin.Shared;

namespace Techfin.Customer.Infra.Messaging;

/// <summary>
/// Classe responsável pela escuta de eventos de transações criadas, realizando o devido débito na conta do cliente e registrando o evento processado.
/// </summary>
/// <param name="serviceProvider">Como o background service roda como um singleton, as dependencias serão obtidas através do service provider</param>
/// <param name="options">acesso as configurações do RabbitMQ</param>
public sealed class TransactionConsumer(IServiceProvider serviceProvider, IOptions<RabbitMqSettings> options) : BackgroundService
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;
    private readonly RabbitMqSettings _settings = options.Value;

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
            try
            {
                var json = Encoding.UTF8.GetString(ea.Body.ToArray());

                var evt = JsonSerializer.Deserialize<TransactionCreatedEvent>(json);

                if (evt is null)
                {
                    channel.BasicAck(ea.DeliveryTag, false);

                    return;
                }

                await ProcessEvent(evt);

                channel.BasicAck(ea.DeliveryTag, false);
            }
            catch
            {
                channel.BasicNack(ea.DeliveryTag, false, true);
            }
        };


        channel.BasicConsume(queue: _settings.Queue, autoAck: false, consumer: consumer);

        return Task.Delay(Timeout.Infinite, stoppingToken);
    }

    private async Task ProcessEvent(TransactionCreatedEvent evt)
    {
        using var scope = _serviceProvider.CreateScope();
        var customerRepository = scope.ServiceProvider.GetRequiredService<ICustomerRepository>();
        var eventsRepository = scope.ServiceProvider.GetRequiredService<IEventsRepository>();

        var customer = await customerRepository.GetByIdAsync(evt.CustomerId);

        if (customer is null)
            return;

        await customerRepository.UpdateAsync(customer, evt.Amount);

        await eventsRepository.AddAsync(new ProcessedEvents()
        {
            Id = Guid.NewGuid(),
            Json = JsonSerializer.Serialize(evt),
            ProcessedAt = DateTime.Now
        });
    }
}
