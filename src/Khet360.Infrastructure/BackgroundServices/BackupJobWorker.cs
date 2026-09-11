using System;
using Khet360.Domain.Entities.Common;
using Khet360.Domain.Entities.Platform;
using Khet360.Domain.Entities.Tenant;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Khet360.Infrastructure.Persistence;
using Khet360.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Khet360.Infrastructure.BackgroundServices;

public class BackupJobWorker : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<BackupJobWorker> _logger;
    private readonly TimeSpan _checkInterval = TimeSpan.FromMinutes(15);

    public BackupJobWorker(IServiceProvider serviceProvider, ILogger<BackupJobWorker> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Backup Job Worker started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessPendingBackups(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while processing backup jobs.");
            }

            await Task.Delay(_checkInterval, stoppingToken);
        }
    }

    private async Task ProcessPendingBackups(CancellationToken stoppingToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var platformDb = scope.ServiceProvider.GetRequiredService<PlatformDbContext>();
        var backupService = scope.ServiceProvider.GetRequiredService<IBackupService>();

        var pendingJobs = await platformDb.BackupJobs
            .Where(j => j.Status == BackupStatus.Pending)
            .OrderBy(j => j.RequestedAtUtc)
            .ToListAsync(stoppingToken);

        foreach (var job in pendingJobs)
        {
            try
            {
                await backupService.PerformBackupInternalAsync(job.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to process backup job {Id}", job.Id);
            }
        }

        var pendingRestores = await platformDb.RestoreJobs
            .Where(j => j.Status == RestoreStatus.Pending)
            .OrderBy(j => j.RequestedAtUtc)
            .ToListAsync(stoppingToken);

        foreach (var job in pendingRestores)
        {
            try
            {
                await backupService.PerformRestoreInternalAsync(job.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to process restore job {Id}", job.Id);
            }
        }
    }
}
