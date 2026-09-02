using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Khet360.Application.Interfaces;
using Khet360.Domain.Entities;
using Khet360.Domain.Enums;
using Khet360.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Khet360.Infrastructure.Services;

public class MigrationService : IMigrationService
{
    private readonly PlatformDbContext _platformDb;
    private readonly IBackupService _backupService;
    private readonly ILogger<MigrationService> _logger;

    public MigrationService(PlatformDbContext platformDb, IBackupService backupService, ILogger<MigrationService> logger)
    {
        _platformDb = platformDb;
        _backupService = backupService;
        _logger = logger;
    }

    public async Task<bool> MigrateTenantAsync(Guid tenantId, string targetEnvironment)
    {
        // This method is now a high-level request trigger.
        // The actual migration is handled asynchronously by MigrationJobWorker.

        var tenant = await _platformDb.Tenants.FindAsync(tenantId);
        if (tenant == null) throw new KeyNotFoundException("Tenant not found.");

        // For the purpose of this implementation, we map targetEnvironment to IsolationTier
        // In a real app, this would be a more explicit choice in the UI.
        IsolationTier targetTier = targetEnvironment.Contains("Dedicated", StringComparison.OrdinalIgnoreCase)
            ? IsolationTier.Dedicated
            : IsolationTier.Isolated;

        if (tenant.Tier == targetTier)
        {
            _logger.LogWarning("Tenant {TenantId} is already in tier {Tier}.", tenantId, targetTier);
            return true;
        }

        var job = new PlatformMigrationJob
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            SourceTier = tenant.Tier,
            TargetTier = targetTier,
            RequestedAtUtc = DateTime.UtcNow,
            Status = MigrationStatus.Pending
        };

        _platformDb.MigrationJobs.Add(job);
        await _platformDb.SaveChangesAsync();

        _logger.LogInformation("Migration job {JobId} created for tenant {TenantId} to tier {Tier}.", job.Id, tenantId, targetTier);

        return true;
    }

    public async Task StartMigrationAsync(Guid jobId)
    {
        var job = await _platformDb.MigrationJobs.FindAsync(jobId);
        if (job == null) throw new KeyNotFoundException("Migration job not found.");

        try
        {
            job.Status = MigrationStatus.InProgress;
            await _platformDb.SaveChangesAsync();

            _logger.LogInformation("Starting migration for job {JobId}. Requesting snapshot...", jobId);

            // Trigger the backup as a prerequisite for migration
            var backupJobId = await _backupService.RequestBackupAsync(job.TenantId);
            job.BackupJobId = backupJobId;
            await _platformDb.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            job.Status = MigrationStatus.Failed;
            job.ErrorMessage = ex.Message;
            await _platformDb.SaveChangesAsync();
            _logger.LogError(ex, "Failed to start migration for job {JobId}.", jobId);
        }
    }

    public async Task CompleteTransferAsync(Guid jobId)
    {
        var job = await _platformDb.MigrationJobs.FindAsync(jobId);
        if (job == null) throw new KeyNotFoundException("Migration job not found.");

        var tenant = await _platformDb.Tenants.FindAsync(job.TenantId);
        if (tenant == null) throw new KeyNotFoundException("Tenant not found.");

        try
        {
            _logger.LogInformation("Executing database transfer for job {JobId}...", jobId);

            // 1. Retrieve Backup Info
            if (job.BackupJobId == null) throw new Exception("No associated backup job found.");
            var backupJob = await _platformDb.BackupJobs.FindAsync(job.BackupJobId.Value);
            if (backupJob == null || backupJob.Status != BackupStatus.Completed)
            {
                throw new Exception("Associated backup job is not completed.");
            }

            // 2. Perform DB Transfer (Mocked)
            // In production:
            // - Fetch backup file from S3 (backupJob.BackupFileKey)
            // - Connect to Target Server (based on job.TargetTier)
            // - Restore DB: RESTORE DATABASE [KhetLinQ_{tenant.Slug}] FROM DISK = ...

            _logger.LogInformation("Restoring database KhetLinQ_{Slug} to {Tier} server...", tenant.Slug, job.TargetTier);

            // 3. Update Registry
            tenant.Tier = job.TargetTier;

            // Update connection string based on tier
            // This is a simplified mock of connection string resolution
            tenant.ConnectionString = job.TargetTier == IsolationTier.Dedicated
                ? "Server=DedicatedServer;Database=KhetLinQ_" + tenant.Slug + ";User Id=sa;Password=Password123!;"
                : "Server=SharedServer;Database=KhetLinQ_" + tenant.Slug + ";User Id=sa;Password=Password123!;";

            _platformDb.Tenants.Update(tenant);

            job.Status = MigrationStatus.Completed;
            job.CompletedAtUtc = DateTime.UtcNow;

            await _platformDb.SaveChangesAsync();
            _logger.LogInformation("Migration job {JobId} completed successfully.", jobId);
        }
        catch (Exception ex)
        {
            job.Status = MigrationStatus.Failed;
            job.ErrorMessage = ex.Message;
            await _platformDb.SaveChangesAsync();
            _logger.LogError(ex, "Error during transfer phase for job {JobId}.", jobId);
        }
    }
}
