using Khet360.Application.Interfaces;
using Khet360.Domain.Entities;
using Khet360.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Khet360.Infrastructure.Services;

public class TenantManagementService : ITenantManagementService
{
    private readonly PlatformDbContext _platformDb;
    private readonly ITenantProvisioningService _provisioningService;

    public TenantManagementService(PlatformDbContext platformDb, ITenantProvisioningService provisioningService)
    {
        _platformDb = platformDb;
        _provisioningService = provisioningService;
    }

    public async Task<Tenant> CreateTenantAsync(string name, string slug, Guid subscriptionPlanId, string connectionString)
    {
        if (await _platformDb.Tenants.AnyAsync(t => t.Slug == slug))
        {
            throw new InvalidOperationException($"Tenant with slug {slug} already exists.");
        }

        var tenant = new Tenant
        {
            Id = Guid.NewGuid(),
            Name = name,
            Slug = slug,
            ConnectionString = connectionString,
            SubscriptionPlanId = subscriptionPlanId,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _platformDb.Tenants.Add(tenant);
        await _platformDb.SaveChangesAsync();

        // Provision the tenant database
        await _provisioningService.ProvisionTenantAsync(tenant.Id, tenant.Slug, tenant.ConnectionString);

        return tenant;
    }

    public async Task<bool> ActivateTenantAsync(Guid tenantId)
    {
        var tenant = await _platformDb.Tenants.FindAsync(tenantId);
        if (tenant == null) return false;

        tenant.IsActive = true;
        await _platformDb.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeactivateTenantAsync(Guid tenantId)
    {
        var tenant = await _platformDb.Tenants.FindAsync(tenantId);
        if (tenant == null) return false;

        tenant.IsActive = false;
        await _platformDb.SaveChangesAsync();
        return true;
    }

    public async Task<Tenant?> GetTenantBySlugAsync(string slug)
    {
        return await _platformDb.Tenants.FirstOrDefaultAsync(t => t.Slug == slug);
    }
}
