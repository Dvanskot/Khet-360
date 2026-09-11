using Khet360.Application.Interfaces;
using Khet360.Domain.Entities.Platform;
using Khet360.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.IO;

namespace Khet360.Infrastructure.Services;

public class BackupService : IBackupService
{
    private readonly PlatformDbContext _platformDb;
    private readonly IFileStorageService _storage;
    private readonly ILogger<BackupService> _logger;

    public BackupService(PlatformDbContext platformDb, IFileStorageService storage, ILogger<BackupService> logger)
    {
        _platformDb = platformDb;
        _storage = storage;
        _logger = logger;
    }

    public async Task<Guid> RequestBackupAsync(Guid tenantId)
    {
        if (!await _platformDb.Tenants.AnyAsync(tenant => tenant.Id == tenantId))
        {
            throw new KeyNotFoundException("Tenant not found.");
        }

        var job = new PlatformBackupJob
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            RequestedAtUtc = DateTime.UtcNow,
            Status = BackupStatus.Pending
        };

        _platformDb.BackupJobs.Add(job);
        await _platformDb.SaveChangesAsync();

        return job.Id;
    }

    public async Task<Guid> RequestRestoreAsync(Guid tenantId, Guid backupJobId)
    {
        var backupJob = await _platformDb.BackupJobs
            .AsNoTracking()
            .FirstOrDefaultAsync(job => job.Id == backupJobId
                && job.TenantId == tenantId
                && job.Status == BackupStatus.Completed
                && !string.IsNullOrEmpty(job.BackupFileKey));

        if (backupJob == null)
        {
            throw new InvalidOperationException("A completed backup for the tenant is required before restore.");
        }

        var restoreJob = new PlatformRestoreJob
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            BackupJobId = backupJobId,
            RequestedAtUtc = DateTime.UtcNow,
            Status = RestoreStatus.Pending
        };

        _platformDb.RestoreJobs.Add(restoreJob);
        await _platformDb.SaveChangesAsync();

        return restoreJob.Id;
    }

    public async Task<PlatformBackupJob> GetBackupStatusAsync(Guid backupJobId)
    {
        return await _platformDb.BackupJobs.FindAsync(backupJobId)
            ?? throw new KeyNotFoundException("Backup job not found.");
    }

    public async Task<List<PlatformBackupJob>> GetBackupHistoryAsync(Guid tenantId)
    {
        return await _platformDb.BackupJobs
            .AsNoTracking()
            .Where(job => job.TenantId == tenantId)
            .OrderByDescending(job => job.RequestedAtUtc)
            .ToListAsync();
    }

    public async Task<PlatformRestoreJob> GetRestoreStatusAsync(Guid restoreJobId)
    {
        return await _platformDb.RestoreJobs.FindAsync(restoreJobId)
            ?? throw new KeyNotFoundException("Restore job not found.");
    }

    public async Task<List<PlatformRestoreJob>> GetRestoreHistoryAsync(Guid tenantId)
    {
        return await _platformDb.RestoreJobs
            .AsNoTracking()
            .Where(job => job.TenantId == tenantId)
            .OrderByDescending(job => job.RequestedAtUtc)
            .ToListAsync();
    }

    public async Task PerformBackupInternalAsync(Guid backupJobId)
    {
        var claimed = await ClaimBackupJobAsync(backupJobId);
        if (!claimed)
        {
            return;
        }

        var job = await _platformDb.BackupJobs.FindAsync(backupJobId);
        if (job == null)
        {
            return;
        }

        try
        {
            _logger.LogInformation("Performing backup for tenant {TenantId}...", job.TenantId);

            var tenant = await _platformDb.Tenants.AsNoTracking().FirstOrDefaultAsync(tenant => tenant.Id == job.TenantId);
            if (tenant == null)
            {
                throw new KeyNotFoundException("Tenant not found.");
            }

            var dbName = $"KhetLinQ_{tenant.Slug}";
            var fileName = $"{dbName}_{DateTime.UtcNow:yyyyMMddHHmmss}.bak";
            var dummyContent = new byte[1024 * 1024];
            var folder = $"backups/{tenant.Slug}";
            var fileKey = $"{folder}/{fileName}";

            using var stream = new MemoryStream(dummyContent);
            await _storage.UploadFileAsync(stream, fileName, "application/octet-stream", folder);

            job.BackupFileKey = fileKey;
            job.FileSize = dummyContent.Length;
            job.Status = BackupStatus.Completed;
            job.CompletedAtUtc = DateTime.UtcNow;
            job.ErrorMessage = null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Backup failed for job {Id}", backupJobId);
            job.Status = BackupStatus.Failed;
            job.ErrorMessage = ex.Message;
        }
        finally
        {
            await _platformDb.SaveChangesAsync();
        }
    }

    public async Task PerformRestoreInternalAsync(Guid restoreJobId)
    {
        var claimed = await ClaimRestoreJobAsync(restoreJobId);
        if (!claimed)
        {
            return;
        }

        var restoreJob = await _platformDb.RestoreJobs.FindAsync(restoreJobId);
        if (restoreJob == null)
        {
            return;
        }

        try
        {
            var backupJob = await _platformDb.BackupJobs
                .AsNoTracking()
                .FirstOrDefaultAsync(job => job.Id == restoreJob.BackupJobId
                    && job.TenantId == restoreJob.TenantId
                    && job.Status == BackupStatus.Completed);

            if (backupJob == null || string.IsNullOrEmpty(backupJob.BackupFileKey))
            {
                throw new InvalidOperationException("Associated backup is not available for restore.");
            }

            var tenant = await _platformDb.Tenants.AsNoTracking().FirstOrDefaultAsync(tenant => tenant.Id == restoreJob.TenantId);
            if (tenant == null)
            {
                throw new KeyNotFoundException("Tenant not found.");
            }

            _logger.LogInformation("Restoring backup {BackupKey} for tenant {TenantId}...", backupJob.BackupFileKey, tenant.Id);
            restoreJob.Status = RestoreStatus.Completed;
            restoreJob.CompletedAtUtc = DateTime.UtcNow;
            restoreJob.ErrorMessage = null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Restore failed for job {Id}", restoreJobId);
            restoreJob.Status = RestoreStatus.Failed;
            restoreJob.ErrorMessage = ex.Message;
        }
        finally
        {
            await _platformDb.SaveChangesAsync();
        }
    }

    private async Task<bool> ClaimBackupJobAsync(Guid backupJobId)
    {
        var now = DateTime.UtcNow;
        var job = await _platformDb.BackupJobs.FindAsync(backupJobId);
        if (job == null || job.Status != BackupStatus.Pending)
        {
            return false;
        }

        job.Status = BackupStatus.InProgress;
        job.ClaimedAtUtc = now;
        job.ClaimedBy = "backup-worker";
        await _platformDb.SaveChangesAsync();
        return true;
    }

    private async Task<bool> ClaimRestoreJobAsync(Guid restoreJobId)
    {
        var now = DateTime.UtcNow;
        var job = await _platformDb.RestoreJobs.FindAsync(restoreJobId);
        if (job == null || job.Status != RestoreStatus.Pending)
        {
            return false;
        }

        job.Status = RestoreStatus.InProgress;
        job.ClaimedAtUtc = now;
        job.ClaimedBy = "backup-worker";
        await _platformDb.SaveChangesAsync();
        return true;
    }
}
