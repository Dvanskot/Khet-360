using Khet360.Domain.Entities;
using Khet360.Domain.Enums;
using Khet360.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace Khet360.Infrastructure.BackgroundServices;

public class SlaEscalationWorker : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<SlaEscalationWorker> _logger;
    private readonly TimeSpan _checkInterval = TimeSpan.FromMinutes(30);

    public SlaEscalationWorker(IServiceProvider serviceProvider, ILogger<SlaEscalationWorker> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("SLA Escalation Worker started.");

        var factory = new ConnectionFactory();
        factory.HostName = "localhost";
        factory.UserName = "guest";
        factory.Password = "guest";

        try
        {
            using var connection = await factory.CreateConnectionAsync(stoppingToken);
            using var channel = await connection.CreateChannelAsync(cancellationToken: stoppingToken);

            await channel.ExchangeDeclareAsync("khet360_events", ExchangeType.Fanout, cancellationToken: stoppingToken);
            var queueDeclareResult = await channel.QueueDeclareAsync(cancellationToken: stoppingToken);
            var queueName = queueDeclareResult.QueueName;
            await channel.QueueBindAsync(queueName, "khet360_events", "", cancellationToken: stoppingToken);

            var consumer = new AsyncEventingBasicConsumer(channel);
            consumer.ReceivedAsync += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                _logger.LogInformation("SLA Worker received event: {Message}", message);
                await Task.CompletedTask;
            };

            await channel.BasicConsumeAsync(queueName, autoAck: true, consumer: consumer, cancellationToken: stoppingToken);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await ProcessSlaEscalations();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while processing SLA escalations.");
                }

                await Task.Delay(_checkInterval, stoppingToken);
            }
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "SLA Escalation Worker failed to initialize RabbitMQ connection.");
        }
    }

    private async Task ProcessSlaEscalations()
    {
        using var scope = _serviceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<TenantDbContext>();

        var pendingWork = await db.WorkItems
            .Where(wi => wi.Status != WorkItemStatus.Completed && wi.Status != WorkItemStatus.Cancelled)
            .ToListAsync();

        foreach (var wi in pendingWork)
        {
            var currentSla = CalculateSlaStatus(wi);
            if (currentSla != wi.SlaStatus)
            {
                var oldStatus = wi.SlaStatus;
                wi.SlaStatus = currentSla;
                wi.LastSlaUpdate = DateTime.UtcNow;

                db.WorkItemHistories.Add(new WorkItemHistory
                {
                    Id = Guid.NewGuid(),
                    WorkItemId = wi.Id,
                    Note = $"SLA Status transitioned from {oldStatus} to {currentSla}",
                    TimestampUtc = DateTime.UtcNow
                });

                if (currentSla == SlaStatus.Breached)
                {
                    _logger.LogWarning("SLA BREACH: WorkItem {Id} ({NextAction}) has breached its SLA!", wi.Id, wi.NextAction);
                }
                else if (currentSla == SlaStatus.Warning)
                {
                    _logger.LogInformation("SLA Warning: WorkItem {Id} is approaching its due date.", wi.Id);
                }
            }
        }

        await db.SaveChangesAsync();
    }

    private SlaStatus CalculateSlaStatus(WorkItem wi)
    {
        var now = DateTime.UtcNow;
        if (now > wi.DueDate)
            return SlaStatus.Breached;

        var timeUntilDue = wi.DueDate - now;

        var warningThreshold = wi.Priority switch
        {
            WorkItemPriority.Critical => TimeSpan.FromHours(4),
            WorkItemPriority.High => TimeSpan.FromHours(8),
            WorkItemPriority.Medium => TimeSpan.FromHours(24),
            _ => TimeSpan.FromHours(48)
        };

        if (timeUntilDue <= warningThreshold)
            return SlaStatus.Warning;

        return SlaStatus.OnTrack;
    }
}
