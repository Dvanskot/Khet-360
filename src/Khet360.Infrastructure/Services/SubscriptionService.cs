using System;
using System.Linq;
using System.Threading.Tasks;
using Khet360.Application.Interfaces;
using Khet360.Domain.Entities;
using Khet360.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Khet360.Infrastructure.Services;

public class SubscriptionService : ISubscriptionService
{
    private readonly PlatformDbContext _platformDb;
    private readonly ILogger<SubscriptionService> _logger;

    public SubscriptionService(PlatformDbContext platformDb, ILogger<SubscriptionService> logger)
    {
        _platformDb = platformDb;
        _logger = logger;
    }

    public async Task<SubscriptionStatusDto> GetSubscriptionStatusAsync(Guid tenantId)
    {
        var tenant = await _platformDb.Tenants
            .Include(t => t.SubscriptionPlan)
            .FirstOrDefaultAsync(t => t.Id == tenantId);

        if (tenant == null)
            throw new KeyNotFoundException($"Tenant with ID {tenantId} not found.");

        return new SubscriptionStatusDto(
            tenant.Id,
            tenant.SubscriptionPlan.Name,
            tenant.SubscriptionStatus,
            tenant.SubscriptionStartDate,
            tenant.SubscriptionEndDate,
            tenant.TrialEndDate
        );
    }

    public async Task ChangePlanAsync(Guid tenantId, Guid newPlanId)
    {
        var tenant = await _platformDb.Tenants.FindAsync(tenantId);
        if (tenant == null)
            throw new KeyNotFoundException($"Tenant with ID {tenantId} not found.");

        var plan = await _platformDb.SubscriptionPlans.FindAsync(newPlanId);
        if (plan == null)
            throw new KeyNotFoundException($"Subscription plan with ID {newPlanId} not found.");

        tenant.SubscriptionPlanId = newPlanId;
        tenant.UpdatedAt = DateTime.UtcNow;

        _logger.LogInformation("Tenant {TenantId} changed plan to {PlanName}", tenantId, plan.Name);
        await _platformDb.SaveChangesAsync();
    }

    public async Task ActivateSubscriptionAsync(Guid tenantId, int durationMonths)
    {
        var tenant = await _platformDb.Tenants.FindAsync(tenantId);
        if (tenant == null)
            throw new KeyNotFoundException($"Tenant with ID {tenantId} not found.");

        tenant.SubscriptionStatus = SubscriptionStatus.Active;
        tenant.SubscriptionStartDate = DateTime.UtcNow;
        tenant.SubscriptionEndDate = DateTime.UtcNow.AddMonths(durationMonths);
        tenant.UpdatedAt = DateTime.UtcNow;

        await _platformDb.SaveChangesAsync();
    }

    public async Task<bool> ValidateLimitAsync(Guid tenantId, string entitlementCode, decimal currentUsage)
    {
        var tenant = await _platformDb.Tenants
            .Include(t => t.SubscriptionPlan)
            .ThenInclude(p => p.Entitlements)
            .FirstOrDefaultAsync(t => t.Id == tenantId);

        if (tenant == null) return false;

        var entitlement = tenant.SubscriptionPlan.Entitlements
            .FirstOrDefault(e => e.Code == entitlementCode && e.IsActive);

        if (entitlement == null) return false;

        // If LimitValue is 0, it might be a boolean entitlement (which we already checked if it exists)
        // or it's a hard limit of 0. Usually, if it's a limit, it's > 0.
        if (entitlement.LimitValue <= 0) return true; // Treat as boolean entitlement

        return currentUsage < entitlement.LimitValue;
    }
}
