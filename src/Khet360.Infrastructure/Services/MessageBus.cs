using Khet360.Application.Interfaces;
using RabbitMQ.Client;
using System;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace Khet360.Infrastructure.Services;

public class MessageBus : IMessageBus, IAsyncDisposable
{
    private readonly IConnection _connection;
    private readonly IChannel _channel;

    public MessageBus(IConfiguration configuration)
    {
        var factory = new ConnectionFactory()
        {
            HostName = configuration["RabbitMq:Host"] ?? "localhost",
            UserName = configuration["RabbitMq:User"] ?? "guest",
            Password = configuration["RabbitMq:Pass"] ?? "guest"
        };

        // Sync initialization for constructor - not ideal but required for current DI setup
        _connection = factory.CreateConnectionAsync().GetAwaiter().GetResult();
        _channel = _connection.CreateChannelAsync().GetAwaiter().GetResult();

        _channel.ExchangeDeclareAsync(exchange: "khet360_events", type: ExchangeType.Fanout).GetAwaiter().GetResult();
    }

    public async Task PublishAsync<T>(T message) where T : class
    {
        var json = JsonSerializer.Serialize(message);
        await PublishRawAsync(json);
    }

    public async Task PublishRawAsync(string content)
    {
        var body = Encoding.UTF8.GetBytes(content);

        await _channel.BasicPublishAsync(
            exchange: "khet360_events",
            routingKey: "",
            body: body);
    }

    public async ValueTask DisposeAsync()
    {
        if (_channel != null) await _channel.CloseAsync();
        if (_connection != null) await _connection.CloseAsync();
    }
}
