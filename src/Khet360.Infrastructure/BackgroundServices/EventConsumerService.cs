using Khet360.Infrastructure.Persistence;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using Khet360.Domain.Entities.Common;
using Khet360.Domain.Entities.Platform;
using Khet360.Domain.Entities.Tenant;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;

namespace Khet360.Infrastructure.BackgroundServices;

public class EventConsumerService : BackgroundService
{
    private readonly ILogger<EventConsumerService> _logger;
    private readonly IConfiguration _configuration;
    private readonly IServiceProvider _serviceProvider;
    private IConnection? _connection;
    private IChannel? _channel;

    public EventConsumerService(ILogger<EventConsumerService> logger, IConfiguration configuration, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _configuration = configuration;
        _serviceProvider = serviceProvider;
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

            await _channel.ExchangeDeclareAsync(exchange: "khet360_events", type: ExchangeType.Fanout, cancellationToken: stoppingToken);
            var queueDeclareResult = await _channel.QueueDeclareAsync(cancellationToken: stoppingToken);
            var queueName = queueDeclareResult.QueueName;
            await _channel.QueueBindAsync(queue: queueName, exchange: "khet360_events", routingKey: "", cancellationToken: stoppingToken);

            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.ReceivedAsync += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var deliveryTag = ea.DeliveryTag;

                _logger.LogInformation("Received event: {Message}", message);

                try
                {
                    using var scope = _serviceProvider.CreateScope();
                    var db = scope.ServiceProvider.GetRequiredService<TenantDbContext>();

                    var messageId = ExtractMessageId(message);

                    if (string.IsNullOrEmpty(messageId))
                    {
                        _logger.LogWarning("Received event without a valid MessageId. Skipping idempotency check.");
                        await RouteEvent(message);
                        await _channel.BasicAckAsync(deliveryTag, false);
                        return;
                    }

                    using var transaction = await db.Database.BeginTransactionAsync();
                    try
                    {
                        var processed = await db.InboxMessages
                            .AnyAsync(m => m.MessageId == messageId);

                        if (!processed)
                        {
                            await RouteEvent(message);

                            db.InboxMessages.Add(new InboxMessage
                            {
                                Id = Guid.NewGuid(),
                                MessageId = messageId,
                                ProcessedAt = DateTime.UtcNow
                            });
                            await db.SaveChangesAsync();
                        }
                        else
                        {
                            _logger.LogInformation("Event {MessageId} already processed. Skipping.", messageId);
                        }

                        await transaction.CommitAsync();
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        _logger.LogError(ex, "Error processing event {MessageId}. Transaction rolled back.", messageId);
                        await _channel.BasicNackAsync(deliveryTag, false, requeue: true);
                        return;
                    }

                    await _channel.BasicAckAsync(deliveryTag, false);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Unhandled error processing event. Nacking message.");
                    await _channel.BasicNackAsync(deliveryTag, false, requeue: true);
                }
            };

            await _channel.BasicConsumeAsync(queue: queueName, autoAck: false, consumer: consumer, cancellationToken: stoppingToken);

            _logger.LogInformation("Event Consumer Service is listening for events with Inbox Pattern reliability.");

            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000, stoppingToken);
            }
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "Event Consumer Service failed to initialize RabbitMQ connection.");
        }
    }

    private async Task RouteEvent(string message)
    {
        if (message.Contains("LeadConvertedEvent"))
        {
            ProcessLeadConverted(message);
        }
        else if (message.Contains("FuneralCaseOpenedEvent"))
        {
            ProcessFuneralCaseOpened(message);
        }
        else if (message.Contains("SlaBreachedEvent"))
        {
            ProcessSlaBreach(message);
        }
        await Task.CompletedTask;
    }

    private string ExtractMessageId(string message)
    {
        try
        {
            using var document = JsonDocument.Parse(message);
            if (document.RootElement.TryGetProperty("MessageId", out var messageIdElement))
            {
                return messageIdElement.GetString() ?? string.Empty;
            }
        }
        catch (JsonException)
        {
        }
        return string.Empty;
    }

    private void ProcessLeadConverted(string message)
    {
        _logger.LogInformation("Processing LeadConvertedEvent: triggering welcome sequence.");
    }

    private void ProcessFuneralCaseOpened(string message)
    {
        _logger.LogInformation("Processing FuneralCaseOpenedEvent: initializing operational checklist.");
    }

    private void ProcessSlaBreach(string message)
    {
        _logger.LogWarning("Processing SlaBreachedEvent: sending priority alert to manager.");
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_channel != null) await _channel.CloseAsync();
        if (_connection != null) await _connection.CloseAsync();
        await base.StopAsync(cancellationToken);
    }
}
