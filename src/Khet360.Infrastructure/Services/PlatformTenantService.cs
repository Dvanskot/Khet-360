using System;
using System.Linq;
using System.Threading.Tasks;
using Khet360.Application.Dtos;
using Khet360.Application.Interfaces;
using Khet360.Domain.Entities.Platform;
using Khet360.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Khet360.Infrastructure.Services;

public class PlatformTenantService : IPlatformTenantService
{
    private readonly PlatformDbContext _platformDb;
    private readonly IPlatformCacheService _cache;
    private readonly IPlatformAuditService _auditService;
    private readonly ILogger<PlatformTenantService> _logger;

    public PlatformTenantService(PlatformDbContext platformDb, IPlatformCacheService cache, IPlatformAuditService auditService, ILogger<PlatformTenantService> logger)
    {
        _platformDb = platformDb;
        _cache = cache;
        _auditService = auditService;
        _logger = logger;
    }

    public async Task<TenantDetailDto> GetTenantDetailsAsync(Guid tenantId)
    {
        var tenant = await _platformDb.Tenants
            .AsNoTracking()
            .Include(t => t.SubscriptionPlan)
            .FirstOrDefaultAsync(t => t.Id == tenantId);

        if (tenant == null)
            throw new KeyNotFoundException($"Tenant with ID {tenantId} not found.");

        return new TenantDetailDto(
            tenant.Id,
            tenant.Name,
            tenant.Slug,
            tenant.SubscriptionStatus,
            tenant.SubscriptionPlan.Category,
            tenant.SubscriptionPlan.Name,
            tenant.Tier,
            tenant.IsActive,
            tenant.SubscriptionStartDate,
            tenant.SubscriptionEndDate,
            tenant.TrialEndDate,
            tenant.CreatedAt,
            tenant.UpdatedAt
        );
    }

    public async Task<PagedResult<TenantListDto>> SearchTenantsAsync(TenantSearchFilter filter)
    {
        var query = _platformDb.Tenants
            .AsNoTracking()
            .Include(t => t.SubscriptionPlan)
            .AsQueryable();

        if (!string.IsNullOrEmpty(filter.SearchTerm))
        {
            var term = filter.SearchTerm.ToLower();
            query = query.Where(t => t.Name.ToLower().Contains(term) || t.Slug.ToLower().Contains(term));
        }

        if (filter.Status.HasValue)
            query = query.Where(t => t.SubscriptionStatus == filter.Status.Value);

        if (filter.Tier.HasValue)
            query = query.Where(t => t.Tier == filter.Tier.Value);

        if (filter.IsActive.HasValue)
            query = query.Where(t => t.IsActive == filter.IsActive.Value);

        if (filter.PlanCategory.HasValue)
            query = query.Where(t => t.SubscriptionPlan.Category == filter.PlanCategory.Value);

        var totalCount = await query.CountAsync();
        var items = await query
            .OrderByDescending(t => t.CreatedAt)
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .Select(t => new TenantListDto(
                t.Id,
                t.Name,
                t.Slug,
                t.SubscriptionStatus,
                t.SubscriptionPlan.Name,
                t.Tier,
                t.IsActive,
                t.CreatedAt
            ))
            .ToListAsync();

        return new PagedResult<TenantListDto>(items, totalCount, filter.Page, filter.PageSize);
    }

    public async Task<bool> SuspendTenantAsync(Guid tenantId, string reason, string performedBy)
    {
        var tenant = await _platformDb.Tenants.FindAsync(tenantId);
        if (tenant == null) return false;

        var oldStatus = tenant.SubscriptionStatus;
        tenant.SubscriptionStatus = SubscriptionStatus.Suspended;
        tenant.IsActive = false;
        tenant.UpdatedAt = DateTime.UtcNow;

        await _platformDb.SaveChangesAsync();
        await _cache.InvalidateTenantsAsync();

        await _auditService.LogAuditAsync(tenantId, tenant.Slug, AuditAction.Suspended, nameof(Tenant), tenantId.ToString(), oldStatus.ToString(), SubscriptionStatus.Suspended.ToString(), performedBy, true, reason: reason);

        _logger.LogInformation("Tenant {TenantId} suspended by {User}. Reason: {Reason}", tenantId, performedBy, reason);
        return true;
    }

    public async Task<bool> CancelTenantAsync(Guid tenantId, string reason, string performedBy)
    {
        var tenant = await _platformDb.Tenants.FindAsync(tenantId);
        if (tenant == null) return false;

        var oldStatus = tenant.SubscriptionStatus;
        tenant.SubscriptionStatus = SubscriptionStatus.Canceled;
        tenant.IsActive = false;
        tenant.SubscriptionEndDate = DateTime.UtcNow;
        tenant.UpdatedAt = DateTime.UtcNow;

        await _platformDb.SaveChangesAsync();
        await _cache.InvalidateTenantsAsync();

        await _auditService.LogAuditAsync(tenantId, tenant.Slug, AuditAction.Canceled, nameof(Tenant), tenantId.ToString(), oldStatus.ToString(), SubscriptionStatus.Canceled.ToString(), performedBy, true, reason: reason);

        _logger.LogInformation("Tenant {TenantId} canceled by {User}. Reason: {Reason}", tenantId, performedBy, reason);
        return true;
    }

    public async Task<bool> ReactivateTenantAsync(Guid tenantId, string reason, string performedBy)
    {
        var tenant = await _platformDb.Tenants.FindAsync(tenantId);
        if (tenant == null) return false;

        var now = DateTime.UtcNow;
        var oldStatus = tenant.SubscriptionStatus;
        tenant.SubscriptionStatus = SubscriptionStatus.Active;
        tenant.IsActive = true;
        tenant.SubscriptionStartDate = now;
        tenant.SubscriptionEndDate = now.AddMonths(1);
        tenant.UpdatedAt = now;

        await _platformDb.SaveChangesAsync();
        await _cache.InvalidateTenantsAsync();

        await _auditService.LogAuditAsync(tenantId, tenant.Slug, AuditAction.Reactivated, nameof(Tenant), tenantId.ToString(), oldStatus.ToString(), SubscriptionStatus.Active.ToString(), performedBy, true, reason: reason);

        _logger.LogInformation("Tenant {TenantId} reactivated by {User}. Reason: {Reason}", tenantId, performedBy, reason);
        return true;
    }

    public async Task<bool> UpdateTenantAsync(Guid tenantId, UpdateTenantDto dto, string performedBy)
    {
        var tenant = await _platformDb.Tenants.FindAsync(tenantId);
        if (tenant == null) return false;

        var oldName = tenant.Name;
        var oldPlanId = tenant.SubscriptionPlanId;

        tenant.Name = dto.Name;
        tenant.IsActive = dto.IsActive;
        tenant.UpdatedAt = DateTime.UtcNow;

        if (dto.SubscriptionPlanId.HasValue)
            tenant.SubscriptionPlanId = dto.SubscriptionPlanId.Value;

        await _platformDb.SaveChangesAsync();
        await _cache.InvalidateTenantsAsync();

        var changes = System.Text.Json.JsonSerializer.Serialize(new { Name = new { oldName, dto.Name }, SubscriptionPlanId = new { oldPlanId, dto.SubscriptionPlanId } });
        await _auditService.LogAuditAsync(tenantId, tenant.Slug, AuditAction.Updated, nameof(Tenant), tenantId.ToString(), null, changes, performedBy, true);

        return true;
    }
}
