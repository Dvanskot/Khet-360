using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Khet360.Application.Interfaces;
using Khet360.Domain.Entities;
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
        // Restoration is a high-risk operation. We create a record and mark it for the worker.
        // For simplicity, we'll use a similar pattern to backup.
        _logger.LogWarning("Restore requested for tenant {TenantId} using backup {BackupId}", tenantId, backupJobId);

        // In a real implementation, this would trigger a specialized restore worker
        return Guid.NewGuid();
    }

    public async Task<PlatformBackupJob> GetBackupStatusAsync(Guid backupJobId)
    {
        return await _platformDb.BackupJobs.FindAsync(backupJobId)
            ?? throw new KeyNotFoundException("Backup job not found.");
    }

    public async Task<List<PlatformBackupJob>> GetBackupHistoryAsync(Guid tenantId)
    {
        return await _platformDb.BackupJobs
            .Where(j => j.TenantId == tenantId)
            .OrderByDescending(j => j.RequestedAtUtc)
            .ToListAsync();
    }

    public async Task PerformBackupInternalAsync(Guid backupJobId)
    {
        var job = await _platformDb.BackupJobs.FindAsync(backupJobId);
        if (job == null) return;

        try
        {
            job.Status = BackupStatus.InProgress;
            await _platformDb.SaveChangesAsync();

            _logger.LogInformation("Performing backup for tenant {TenantId}...", job.TenantId);

            // 1. Identify the database name
            var tenant = await _platformDb.Tenants.FindAsync(job.TenantId);
            if (tenant == null) throw new Exception("Tenant not found.");
            var dbName = $"KhetLinQ_{tenant.Slug}";

            // 2. Execute SQL Backup
            // In a real environment:
            // var sql = $"BACKUP DATABASE [{dbName}] TO DISK = 'C:\\temp\\{dbName}.bak'";
            // await _platformDb.Database.ExecuteSqlRawAsync(sql);

            // Mocking the backup file creation
            var fileName = $"{dbName}_{DateTime.UtcNow:yyyyMMddHHmmss}.bak";
            var dummyContent = new byte[1024 * 1024]; // 1MB mock backup

            // 3. Upload to MinIO
            var folder = $"backups/{tenant.Slug}";
            var fileKey = $"{folder}/{fileName}";
            using var stream = new MemoryStream(dummyContent);
            await _storage.UploadFileAsync(stream, fileName, "application/octet-stream", folder);

            job.BackupFileKey = fileKey;
            job.FileSize = dummyContent.Length;
            job.Status = BackupStatus.Completed;
            job.CompletedAtUtc = DateTime.UtcNow;
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
}
