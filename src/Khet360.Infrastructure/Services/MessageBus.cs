using Khet360.Application.Interfaces;
using RabbitMQ.Client;
using System;
using Khet360.Domain.Entities.Common;
using Khet360.Domain.Entities.Platform;
using Khet360.Domain.Entities.Tenant;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Khet360.Infrastructure.Services;

public class MessageBus : IMessageBus, IAsyncDisposable
{
    private readonly ILogger<MessageBus> _logger;
    private IConnection? _connection;
    private IChannel? _channel;
    private readonly string _hostName;
    private readonly string _userName;
    private readonly string _password;
    private bool _initialized;

    public MessageBus(IConfiguration configuration, ILogger<MessageBus> logger)
    {
        _logger = logger;
        _hostName = configuration["RabbitMq:Host"] ?? "localhost";
        _userName = configuration["RabbitMq:User"] ?? "guest";
        _password = configuration["RabbitMq:Pass"] ?? "guest";
    }

    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        if (_initialized) return;

        var factory = new ConnectionFactory()
        {
            HostName = _hostName,
            UserName = _userName,
            Password = _password
        };

        _connection = await factory.CreateConnectionAsync(cancellationToken);
        _channel = await _connection.CreateChannelAsync(cancellationToken: cancellationToken);

        await _channel.ExchangeDeclareAsync(exchange: "khet360_events", type: ExchangeType.Fanout, cancellationToken: cancellationToken);

        _initialized = true;
        _logger.LogInformation("MessageBus initialized and connected to RabbitMQ");
    }

    public async Task PublishAsync<T>(T message) where T : class
    {
        EnsureInitialized();
        var json = JsonSerializer.Serialize(message);
        await PublishRawAsync(json);
    }

    public async Task PublishRawAsync(string content)
    {
        EnsureInitialized();
        var body = Encoding.UTF8.GetBytes(content);

        await _channel!.BasicPublishAsync(
            exchange: "khet360_events",
            routingKey: "",
            body: body);
    }

    private void EnsureInitialized()
    {
        if (!_initialized || _channel == null || _connection == null)
        {
            throw new InvalidOperationException("MessageBus not initialized. Call InitializeAsync first.");
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_channel != null) await _channel.CloseAsync();
        if (_connection != null) await _connection.CloseAsync();
    }
}
