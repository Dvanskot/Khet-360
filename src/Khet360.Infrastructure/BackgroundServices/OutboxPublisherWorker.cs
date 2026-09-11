using System;
using Khet360.Domain.Entities.Common;
using Khet360.Domain.Entities.Platform;
using Khet360.Domain.Entities.Tenant;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Khet360.Application.Interfaces;
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
    private readonly IPlatformCacheService _cache;
    private readonly TimeSpan _checkInterval = TimeSpan.FromSeconds(5);

    public OutboxPublisherWorker(IServiceProvider serviceProvider, ILogger<OutboxPublisherWorker> logger, IPlatformCacheService cache)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _cache = cache;
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
        var tenants = await _cache.GetTenantsAsync();

        // Process tenants in parallel with a degree of parallelism to avoid overwhelming the system
        var semaphore = new SemaphoreSlim(5); // Max 5 concurrent tenant processing
        var tasks = tenants.Select(async tenant =>
        {
            await semaphore.WaitAsync(stoppingToken);
            try
            {
                await ProcessTenantOutbox(tenant, stoppingToken);
            }
            finally
            {
                semaphore.Release();
            }
        });

        await Task.WhenAll(tasks);
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

        if (messages.Count == 0)
        {
            return; // No messages for this tenant, skip
        }

        foreach (var message in messages)
        {
            try
            {
                // Use PublishRawAsync to avoid double-serialization of already stored JSON content
                await messageBus.PublishRawAsync(message.Content);

                message.ProcessedAtUtc = DateTime.UtcNow;
                _logger.LogInformation("Published outbox message {Id} for tenant {Tenant}", message.Id, tenant.Slug);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to publish outbox message {Id}", message.Id);
                message.Error = ex.Message;
                message.RetryCount++;
                message.LastErrorAt = DateTime.UtcNow;
            }
        }

        await db.SaveChangesAsync(stoppingToken);
    }
}
