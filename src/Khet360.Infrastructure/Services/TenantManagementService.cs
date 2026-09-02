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

    public async Task<Tenant> CreateTenantAsync(string name, string slug, Guid subscriptionPlanId, IsolationTier tier)
    {
        if (await _platformDb.Tenants.AnyAsync(t => t.Slug == slug))
        {
            throw new InvalidOperationException($"Tenant with slug {slug} already exists.");
        }

        var plan = await _platformDb.SubscriptionPlans.FindAsync(subscriptionPlanId);
        if (plan == null)
        {
            throw new KeyNotFoundException($"Subscription plan with ID {subscriptionPlanId} not found.");
        }

        var tenantId = Guid.NewGuid();

        // 1. Provision the tenant database first to get the connection string
        var connectionString = await _provisioningService.ProvisionTenantAsync(tenantId, slug, tier);

        // 2. Create the tenant record in the platform database
        var now = DateTime.UtcNow;
        var tenant = new Tenant
        {
            Id = tenantId,
            Name = name,
            Slug = slug,
            ConnectionString = connectionString,
            SubscriptionPlanId = subscriptionPlanId,
            Tier = tier,
            IsActive = true,
            SubscriptionStatus = plan.TrialPeriodDays > 0 ? SubscriptionStatus.Trial : SubscriptionStatus.Active,
            SubscriptionStartDate = now,
            TrialEndDate = plan.TrialPeriodDays > 0 ? now.AddDays(plan.TrialPeriodDays) : null,
            SubscriptionEndDate = plan.TrialPeriodDays > 0 ? now.AddDays(plan.TrialPeriodDays) : now.AddMonths(1),
            CreatedAt = now
        };

        _platformDb.Tenants.Add(tenant);
        await _platformDb.SaveChangesAsync();

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
