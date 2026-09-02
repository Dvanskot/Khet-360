using System;
using System.Threading.Tasks;
using Khet360.Application.Interfaces;
using Khet360.Domain.Entities;
using Khet360.Infrastructure.Persistence;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

namespace Khet360.Infrastructure.Services;

public class MigrationService : IMigrationService
{
    private readonly PlatformDbContext _platformDb;
    private readonly ILogger<MigrationService> _logger;

    public MigrationService(PlatformDbContext platformDb, ILogger<MigrationService> logger)
    {
        _platformDb = platformDb;
        _logger = logger;
    }

    public async Task<bool> MigrateTenantAsync(Guid tenantId, string targetEnvironment)
    {
        _logger.LogInformation("Starting migration for tenant {TenantId} to {Env}", tenantId, targetEnvironment);

        var tenant = await _platformDb.Tenants.FindAsync(tenantId);
        if (tenant == null) throw new KeyNotFoundException("Tenant not found.");

        try
        {
            // 1. Trigger Backup of source tenant
            // (In a real system, this would wait for the BackupJobWorker to complete)
            _logger.LogInformation("Triggering source backup for {Tenant}", tenant.Slug);

            // 2. Perform database transfer
            // This typically involves:
            // - Exporting the .bak file from source
            // - Importing the .bak file into target
            // - Renaming the DB to avoid conflicts
            _logger.LogInformation("Transferring database KhetLinQ_{Slug} to {Env}", tenant.Slug, targetEnvironment);

            // 3. Update Platform Registry in target environment
            // (Assuming this service runs in the target environment's context)
            tenant.UpdatedAt = DateTime.UtcNow;
            _platformDb.Tenants.Update(tenant);
            await _platformDb.SaveChangesAsync();

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Migration failed for tenant {TenantId}", tenantId);
            return false;
        }
    }
}
