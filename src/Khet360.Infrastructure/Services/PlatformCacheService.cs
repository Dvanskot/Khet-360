using Khet360.Application.Interfaces;
using Khet360.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using Khet360.Domain.Entities.Common;
using Khet360.Domain.Entities.Platform;
using Khet360.Domain.Entities.Tenant;
using System.Threading.Tasks;

namespace Khet360.Infrastructure.Services;

public class PlatformCacheService : IPlatformCacheService
{
    private readonly ICacheService _cache;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<PlatformCacheService> _logger;

    private const string TenantsCacheKey = "platform:tenants:all";
    private const string SubscriptionPlansCacheKey = "platform:subscriptionplans:all";
    private const string EntitlementsCacheKey = "platform:entitlements:all";
    private const string TaxYearsCacheKey = "platform:taxyears:all";
    private const string TaxBracketsCacheKey = "platform:taxbrackets:all";
    private const string StatutoryRatesCacheKey = "platform:statutoryrates:all";
    private const string LeaveTypesCacheKey = "platform:leavetypes:all";
    private const string PositionsCacheKey = "platform:positions:all";

    private static readonly TimeSpan DefaultCacheTtl = TimeSpan.FromMinutes(10);

    public PlatformCacheService(ICacheService cache, IServiceProvider serviceProvider, ILogger<PlatformCacheService> logger)
    {
        _cache = cache;
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task<List<Tenant>> GetTenantsAsync()
    {
        return await _cache.GetOrSetAsync(TenantsCacheKey, LoadTenantsAsync, DefaultCacheTtl);
    }

    public async Task<List<SubscriptionPlan>> GetSubscriptionPlansAsync()
    {
        return await _cache.GetOrSetAsync(SubscriptionPlansCacheKey, LoadSubscriptionPlansAsync, DefaultCacheTtl);
    }

    public async Task<List<Entitlement>> GetEntitlementsAsync()
    {
        return await _cache.GetOrSetAsync(EntitlementsCacheKey, LoadEntitlementsAsync, DefaultCacheTtl);
    }

    public async Task<List<TaxYear>> GetTaxYearsAsync()
    {
        return await _cache.GetOrSetAsync(TaxYearsCacheKey, LoadTaxYearsAsync, DefaultCacheTtl);
    }

    public async Task<List<TaxBracket>> GetTaxBracketsAsync()
    {
        return await _cache.GetOrSetAsync(TaxBracketsCacheKey, LoadTaxBracketsAsync, DefaultCacheTtl);
    }

    public async Task<List<StatutoryRate>> GetStatutoryRatesAsync()
    {
        return await _cache.GetOrSetAsync(StatutoryRatesCacheKey, LoadStatutoryRatesAsync, DefaultCacheTtl);
    }

    public async Task<List<LeaveType>> GetLeaveTypesAsync()
    {
        return await _cache.GetOrSetAsync(LeaveTypesCacheKey, LoadLeaveTypesAsync, DefaultCacheTtl);
    }

    public async Task<List<Position>> GetPositionsAsync()
    {
        return await _cache.GetOrSetAsync(PositionsCacheKey, LoadPositionsAsync, DefaultCacheTtl);
    }

    public async Task InvalidateTenantsAsync()
    {
        await _cache.RemoveAsync(TenantsCacheKey);
    }

    public async Task InvalidateSubscriptionPlansAsync()
    {
        await _cache.RemoveAsync(SubscriptionPlansCacheKey);
    }

    public async Task InvalidateEntitlementsAsync()
    {
        await _cache.RemoveAsync(EntitlementsCacheKey);
    }

    public async Task InvalidateAllAsync()
    {
        await Task.WhenAll(
            _cache.RemoveAsync(TenantsCacheKey),
            _cache.RemoveAsync(SubscriptionPlansCacheKey),
            _cache.RemoveAsync(EntitlementsCacheKey),
            _cache.RemoveAsync(TaxYearsCacheKey),
            _cache.RemoveAsync(TaxBracketsCacheKey),
            _cache.RemoveAsync(StatutoryRatesCacheKey),
            _cache.RemoveAsync(LeaveTypesCacheKey),
            _cache.RemoveAsync(PositionsCacheKey)
        );
    }

    private async Task<List<Tenant>> LoadTenantsAsync()
    {
        using var scope = _serviceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<PlatformDbContext>();
        return await db.Tenants.AsNoTracking().Where(t => t.IsActive).ToListAsync();
    }

    private async Task<List<SubscriptionPlan>> LoadSubscriptionPlansAsync()
    {
        using var scope = _serviceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<PlatformDbContext>();
        return await db.SubscriptionPlans.AsNoTracking().Include(p => p.Entitlements).ToListAsync();
    }

    private async Task<List<Entitlement>> LoadEntitlementsAsync()
    {
        using var scope = _serviceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<PlatformDbContext>();
        return await db.Entitlements.AsNoTracking().ToListAsync();
    }

    private async Task<List<TaxYear>> LoadTaxYearsAsync()
    {
        using var scope = _serviceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<PlatformDbContext>();
        return await db.TaxYears.AsNoTracking().ToListAsync();
    }

    private async Task<List<TaxBracket>> LoadTaxBracketsAsync()
    {
        using var scope = _serviceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<PlatformDbContext>();
        return await db.TaxBrackets.AsNoTracking().ToListAsync();
    }

    private async Task<List<StatutoryRate>> LoadStatutoryRatesAsync()
    {
        using var scope = _serviceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<PlatformDbContext>();
        return await db.StatutoryRates.AsNoTracking().ToListAsync();
    }

    private async Task<List<LeaveType>> LoadLeaveTypesAsync()
    {
        using var scope = _serviceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<PlatformDbContext>();
        return await db.LeaveTypes.AsNoTracking().ToListAsync();
    }

    private async Task<List<Position>> LoadPositionsAsync()
    {
        using var scope = _serviceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<PlatformDbContext>();
        return await db.Positions.AsNoTracking().ToListAsync();
    }
}
