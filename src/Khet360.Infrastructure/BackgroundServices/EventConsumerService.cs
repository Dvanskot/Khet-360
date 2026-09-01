using Khet360.Domain.Events;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace Khet360.Infrastructure.BackgroundServices;

public class EventConsumerService : BackgroundService
{
    private readonly ILogger<EventConsumerService> _logger;
    private readonly IConfiguration _configuration;
    private IConnection? _connection;
    private IChannel? _channel;

    public EventConsumerService(ILogger<EventConsumerService> logger, IConfiguration configuration)
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

            await _channel.ExchangeDeclareAsync(exchange: "khet360_events", type: ExchangeType.Fanout, cancellationToken: stoppingToken);
            var queueDeclareResult = await _channel.QueueDeclareAsync(cancellationToken: stoppingToken);
            var queueName = queueDeclareResult.QueueName;
            await _channel.QueueBindAsync(queue: queueName, exchange: "khet360_events", routingKey: "", cancellationToken: stoppingToken);

            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.ReceivedAsync += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                _logger.LogInformation("Received event: {Message}", message);

                // Basic routing based on content
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
            };

            await _channel.BasicConsumeAsync(queue: queueName, autoAck: true, consumer: consumer, cancellationToken: stoppingToken);

            _logger.LogInformation("Event Consumer Service is listening for events.");

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
