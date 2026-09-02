using System;
using System.Linq;
using System.Threading.Tasks;
using Khet360.Application.Interfaces;
using Khet360.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Khet360.Infrastructure.Services;

public class EntitlementService : IEntitlementService
{
    private readonly PlatformDbContext _platformDb;

    public EntitlementService(PlatformDbContext platformDb)
    {
        _platformDb = platformDb;
    }

    public async Task<bool> IsEntitledAsync(Guid tenantId, string entitlementCode)
    {
        return await _platformDb.Tenants
            .Where(t => t.Id == tenantId)
            .SelectMany(t => t.SubscriptionPlan.Entitlements)
            .AnyAsync(e => e.Code == entitlementCode && e.IsActive);
    }

    public async Task<decimal> GetLimitAsync(Guid tenantId, string entitlementCode)
    {
        var limit = await _platformDb.Tenants
            .Where(t => t.Id == tenantId)
            .SelectMany(t => t.SubscriptionPlan.Entitlements)
            .Where(e => e.Code == entitlementCode && e.IsActive)
            .Select(e => e.LimitValue)
            .FirstOrDefaultAsync();

        return limit;
    }
}
