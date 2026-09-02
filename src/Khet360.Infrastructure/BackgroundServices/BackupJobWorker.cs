using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Khet360.Domain.Entities;
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
                // We call the implementation of IBackupService to perform the actual backup
                // We pass the job ID so it can update the status.
                await backupService.PerformBackupInternalAsync(job.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to process backup job {Id}", job.Id);
            }
        }
    }
}
