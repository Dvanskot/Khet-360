using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace Khet360.Infrastructure.BackgroundServices;

public class NotificationConsumerService : BackgroundService
{
    private readonly ILogger<NotificationConsumerService> _logger;
    private readonly IConfiguration _configuration;
    private IConnection? _connection;
    private IChannel? _channel;

    public NotificationConsumerService(ILogger<NotificationConsumerService> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var factory = new ConnectionFactory()
        {
            HostName = _configuration["RabbitMq:Host"] ?? "localhost",
            UserName = _configuration["RabbitMq:User"] ?? "guest",
            Password = _configuration["RabbitMq:Pass"] ?? "guest"
        };

        try
        {
            _connection = await factory.CreateConnectionAsync(stoppingToken);
            _channel = await _connection.CreateChannelAsync(cancellationToken: stoppingToken);

            await _channel.ExchangeDeclareAsync("khet360_events", ExchangeType.Fanout, cancellationToken: stoppingToken);

            // Each consumer needs its own queue to receive a copy of the fanout event
            var queueDeclareResult = await _channel.QueueDeclareAsync(queue: "notification_queue", durable: true, exclusive: false, autoDelete: false, arguments: null, cancellationToken: stoppingToken);
            var queueName = queueDeclareResult.QueueName;

            await _channel.QueueBindAsync(queueName, "khet360_events", "", cancellationToken: stoppingToken);

            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.ReceivedAsync += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                await HandleNotificationAsync(message);

                await Task.CompletedTask;
            };

            await _channel.BasicConsumeAsync(queueName, autoAck: true, consumer: consumer, cancellationToken: stoppingToken);

            _logger.LogInformation("Notification Consumer Service is active.");

            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000, stoppingToken);
            }
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "Notification Consumer Service failed to initialize.");
        }
    }

    private async Task HandleNotificationAsync(string message)
    {
        if (message.Contains("LeadConvertedEvent"))
        {
            _logger.LogInformation("[NOTIFICATION] Sending welcome email to new customer...");
        }
        else if (message.Contains("SlaBreachedEvent"))
        {
            _logger.LogWarning("[NOTIFICATION] ALERT: Sending urgent SMS to operations manager regarding SLA breach!");
        }

        await Task.CompletedTask;
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_channel != null) await _channel.CloseAsync();
        if (_connection != null) await _connection.CloseAsync();
        await base.StopAsync(cancellationToken);
    }
}
