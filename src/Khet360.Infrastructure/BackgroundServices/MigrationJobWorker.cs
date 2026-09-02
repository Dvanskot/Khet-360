using Khet360.Domain.Entities;
using Khet360.Domain.Enums;
using Khet360.Infrastructure.Persistence;
using Khet360.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Khet360.Infrastructure.BackgroundServices;

public class MigrationJobWorker : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<MigrationJobWorker> _logger;

    public MigrationJobWorker(IServiceProvider serviceProvider, ILogger<MigrationJobWorker> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Migration Job Worker is starting...");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var platformDb = scope.ServiceProvider.GetRequiredService<PlatformDbContext>();
                var migrationService = scope.ServiceProvider.GetRequiredService<MigrationService>();

                // 1. Handle Pending Jobs
                var pendingJobs = await platformDb.MigrationJobs
                    .Where(j => j.Status == MigrationStatus.Pending)
                    .ToListAsync(stoppingToken);

                foreach (var job in pendingJobs)
                {
                    _logger.LogInformation("Processing pending migration job {JobId}...", job.Id);
                    await migrationService.StartMigrationAsync(job.Id);
                }

                // 2. Handle InProgress Jobs (Wait for backup completion)
                var inProgressJobs = await platformDb.MigrationJobs
                    .Where(j => j.Status == MigrationStatus.InProgress)
                    .ToListAsync(stoppingToken);

                foreach (var job in inProgressJobs)
                {
                    if (job.BackupJobId.HasValue)
                    {
                        var backupJob = await platformDb.BackupJobs.FindAsync(job.BackupJobId.Value);
                        if (backupJob != null && backupJob.Status == BackupStatus.Completed)
                        {
                            _logger.LogInformation("Backup completed for job {JobId}. Proceeding to transfer...", job.Id);
                            await migrationService.CompleteTransferAsync(job.Id);
                        }
                    }
                    else
                    {
                        _logger.LogWarning("Migration job {JobId} is InProgress but has no associated BackupJobId.", job.Id);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during migration job processing cycle.");
            }

            // Poll every 30 seconds
            await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
        }
    }
}
