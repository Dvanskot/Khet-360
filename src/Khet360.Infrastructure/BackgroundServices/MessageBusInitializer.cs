using Microsoft.Extensions.Hosting;
using Khet360.Domain.Entities.Tenant;
using Khet360.Domain.Entities.Platform;
using Khet360.Domain.Entities.Common;
using Microsoft.Extensions.Logging;
using Khet360.Infrastructure.Services;
using Khet360.Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Khet360.Infrastructure.BackgroundServices;

public class MessageBusInitializer : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<MessageBusInitializer> _logger;

    public MessageBusInitializer(IServiceProvider serviceProvider, ILogger<MessageBusInitializer> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var messageBus = scope.ServiceProvider.GetRequiredService<IMessageBus>();

        if (messageBus is MessageBus mb)
        {
            try
            {
                await mb.InitializeAsync(stoppingToken);
                _logger.LogInformation("MessageBus initialized successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to initialize MessageBus");
                throw;
            }
        }
    }
}
