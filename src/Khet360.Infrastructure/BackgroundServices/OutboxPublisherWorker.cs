using System;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Khet360.Application.Interfaces;
using Khet360.Domain.Entities;
using Khet360.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Khet360.Infrastructure.BackgroundServices;

public class OutboxPublisherWorker : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<OutboxPublisherWorker> _logger;
    private readonly TimeSpan _checkInterval = TimeSpan.FromSeconds(5);

    public OutboxPublisherWorker(IServiceProvider serviceProvider, ILogger<OutboxPublisherWorker> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Outbox Publisher Worker started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessAllTenantsOutboxes(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while processing outboxes.");
            }

            await Task.Delay(_checkInterval, stoppingToken);
        }
    }

    private async Task ProcessAllTenantsOutboxes(CancellationToken stoppingToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var platformDb = scope.ServiceProvider.GetRequiredService<PlatformDbContext>();
        var tenants = await platformDb.Tenants.ToListAsync(stoppingToken);

        foreach (var tenant in tenants)
        {
            await ProcessTenantOutbox(tenant, stoppingToken);
        }
    }

    private async Task ProcessTenantOutbox(Tenant tenant, CancellationToken stoppingToken)
    {
        using var scope = _serviceProvider.CreateScope();

        var tenantService = scope.ServiceProvider.GetRequiredService<ITenantService>();
        tenantService.SetTenant(tenant);

        var db = scope.ServiceProvider.GetRequiredService<TenantDbContext>();
        var messageBus = scope.ServiceProvider.GetRequiredService<IMessageBus>();

        var messages = await db.OutboxMessages
            .Where(m => m.ProcessedAtUtc == null)
            .OrderBy(m => m.CreatedAtUtc)
            .Take(20)
            .ToListAsync(stoppingToken);

        foreach (var message in messages)
        {
            try
            {
                // We publish as a generic object or use a dispatcher based on EventType
                // For simplicity, we'll publish the content directly.
                await messageBus.PublishAsync(message.Content);

                message.ProcessedAtUtc = DateTime.UtcNow;
                _logger.LogInformation("Published outbox message {Id} for tenant {Tenant}", message.Id, tenant.Slug);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to publish outbox message {Id}", message.Id);
                message.Error = ex.Message;
            }
        }

        await db.SaveChangesAsync(stoppingToken);
    }
}
