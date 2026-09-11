using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Khet360.Application.Dtos;
using Khet360.Application.Interfaces;
using Khet360.Domain.Entities.Platform;
using Khet360.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Khet360.Infrastructure.Services;

public class PlatformFeatureService : IPlatformFeatureService
{
    private readonly PlatformDbContext _platformDb;
    private readonly IPlatformCacheService _cache;
    private readonly IPlatformAuditService _auditService;
    private readonly ILogger<PlatformFeatureService> _logger;

    public PlatformFeatureService(PlatformDbContext platformDb, IPlatformCacheService cache, IPlatformAuditService auditService, ILogger<PlatformFeatureService> logger)
    {
        _platformDb = platformDb;
        _cache = cache;
        _auditService = auditService;
        _logger = logger;
    }

    public async Task<List<FeatureFlagDto>> GetAllFeatureFlagsAsync(Guid? tenantId = null)
    {
        var features = await _platformDb.PlatformFeatures
            .AsNoTracking()
            .Include(f => f.TenantOverrides)
            .ToListAsync();

        return features.Select(f =>
        {
            var tenantOverride = tenantId.HasValue
                ? f.TenantOverrides.FirstOrDefault(o => o.TenantId == tenantId.Value)
                : null;
            var isEnabledForTenant = tenantOverride?.IsEnabled ?? f.IsEnabledGlobally;

            return new FeatureFlagDto(
                f.Id,
                f.Name,
                f.Code,
                f.Description,
                f.Type,
                f.IsEnabledGlobally,
                isEnabledForTenant,
                f.CreatedAt);
        }).ToList();
    }

    public async Task<bool> UpdateGlobalFeatureAsync(Guid featureId, bool isEnabled, string performedBy)
    {
        var feature = await _platformDb.PlatformFeatures.FindAsync(featureId);
        if (feature == null) return false;

        feature.IsEnabledGlobally = isEnabled;
        feature.UpdatedAt = DateTime.UtcNow;

        await _platformDb.SaveChangesAsync();
        await _cache.InvalidateAllAsync();

        await _auditService.LogAuditAsync(Guid.Empty, "platform", AuditAction.FeatureToggled, nameof(PlatformFeature), featureId.ToString(), (!feature.IsEnabledGlobally).ToString(), isEnabled.ToString(), performedBy, true);

        _logger.LogInformation("Feature {FeatureCode} global status updated to {IsEnabled} by {User}", feature.Code, isEnabled, performedBy);
        return true;
    }

    public async Task<bool> SetTenantFeatureOverrideAsync(Guid tenantId, Guid featureId, bool isEnabled, string performedBy)
    {
        var feature = await _platformDb.PlatformFeatures.FindAsync(featureId);
        if (feature == null) return false;

        var existingOverride = await _platformDb.TenantFeatureOverrides
            .FirstOrDefaultAsync(o => o.TenantId == tenantId && o.FeatureId == featureId);

        if (existingOverride != null)
        {
            existingOverride.IsEnabled = isEnabled;
            existingOverride.UpdatedAt = DateTime.UtcNow;
        }
        else
        {
            _platformDb.TenantFeatureOverrides.Add(new TenantFeatureOverride
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                FeatureId = featureId,
                IsEnabled = isEnabled,
                CreatedAt = DateTime.UtcNow
            });
        }

        await _platformDb.SaveChangesAsync();

        var tenant = await _platformDb.Tenants.FindAsync(tenantId);
        await _auditService.LogAuditAsync(tenantId, tenant?.Slug ?? string.Empty, AuditAction.FeatureToggled, nameof(PlatformFeature), featureId.ToString(), null, $"Tenant override: {isEnabled}", performedBy, true);

        return true;
    }

    public async Task<bool> RemoveTenantFeatureOverrideAsync(Guid tenantId, Guid featureId, string performedBy)
    {
        var existingOverride = await _platformDb.TenantFeatureOverrides
            .FirstOrDefaultAsync(o => o.TenantId == tenantId && o.FeatureId == featureId);

        if (existingOverride == null) return false;

        _platformDb.TenantFeatureOverrides.Remove(existingOverride);
        await _platformDb.SaveChangesAsync();

        var tenant = await _platformDb.Tenants.FindAsync(tenantId);
        await _auditService.LogAuditAsync(tenantId, tenant?.Slug ?? string.Empty, AuditAction.FeatureToggled, nameof(PlatformFeature), featureId.ToString(), "true", "inherited", performedBy, true);

        return true;
    }

    public async Task<bool> IsFeatureEnabledForTenantAsync(Guid tenantId, string featureCode)
    {
        var feature = await _platformDb.PlatformFeatures
            .Include(f => f.TenantOverrides)
            .FirstOrDefaultAsync(f => f.Code == featureCode);

        if (feature == null) return false;

        if (!feature.IsEnabledGlobally)
        {
            var override_ = feature.TenantOverrides.FirstOrDefault(o => o.TenantId == tenantId);
            if (override_ != null) return override_.IsEnabled;
            return false;
        }

        var tenantOverride = feature.TenantOverrides.FirstOrDefault(o => o.TenantId == tenantId);
        if (tenantOverride != null) return tenantOverride.IsEnabled;

        return true;
    }
}
