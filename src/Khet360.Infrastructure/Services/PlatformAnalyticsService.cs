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

public class PlatformAnalyticsService : IPlatformAnalyticsService
{
    private readonly PlatformDbContext _platformDb;
    private readonly ILogger<PlatformAnalyticsService> _logger;

    public PlatformAnalyticsService(PlatformDbContext platformDb, ILogger<PlatformAnalyticsService> logger)
    {
        _platformDb = platformDb;
        _logger = logger;
    }

    public async Task<PlatformOverviewDto> GetPlatformHealthAsync()
    {
        var tenants = await _platformDb.Tenants
            .AsNoTracking()
            .Include(t => t.SubscriptionPlan)
            .ToListAsync();

        var activeTenants = tenants.Where(t => t.IsActive).ToList();
        var activeCount = activeTenants.Count;
        var trialCount = tenants.Count(t => t.SubscriptionStatus == SubscriptionStatus.Trial);
        var suspendedCount = tenants.Count(t => t.SubscriptionStatus == SubscriptionStatus.Suspended);
        var canceledCount = tenants.Count(t => t.SubscriptionStatus == SubscriptionStatus.Canceled);

        var now = DateTime.UtcNow;
        var monthlyRevenue = activeTenants
            .Where(t => t.SubscriptionStatus == SubscriptionStatus.Active)
            .Sum(t => t.SubscriptionPlan.MonthlyPrice);

        var annualRevenue = activeTenants
            .Where(t => t.SubscriptionStatus == SubscriptionStatus.Active)
            .Sum(t => t.SubscriptionPlan.AnnualPrice);

        return new PlatformOverviewDto(
            IsHealthy: activeTenants.All(t => t.SubscriptionStatus is SubscriptionStatus.Active or SubscriptionStatus.Trial),
            TotalActiveTenants: activeCount,
            TotalTrialingTenants: trialCount,
            TotalSuspendedTenants: suspendedCount,
            TotalCanceledTenants: canceledCount,
            MonthlyRecurringRevenue: monthlyRevenue,
            AnnualRecurringRevenue: annualRevenue,
            MeasuredAt: now
        );
    }

    public async Task<RevenueAnalyticsDto> GetRevenueAnalyticsAsync()
    {
        var tenants = await _platformDb.Tenants
            .AsNoTracking()
            .Include(t => t.SubscriptionPlan)
            .ToListAsync();

        var activeTenants = tenants.Where(t => t.IsActive && t.SubscriptionStatus == SubscriptionStatus.Active).ToList();
        var monthlyRevenue = activeTenants.Sum(t => t.SubscriptionPlan.MonthlyPrice);
        var annualRevenue = activeTenants.Sum(t => t.SubscriptionPlan.AnnualPrice);
        var avgRevenue = activeTenants.Any() ? monthlyRevenue / activeTenants.Count : 0;

        var revenueByPlan = activeTenants
            .GroupBy(t => t.SubscriptionPlan.Name)
            .Select(g => new RevenueByPlanDto(g.Key, g.Sum(t => t.SubscriptionPlan.MonthlyPrice), g.Count()))
            .OrderByDescending(r => r.Revenue)
            .ToList();

        return new RevenueAnalyticsDto(monthlyRevenue, annualRevenue, avgRevenue, revenueByPlan);
    }

    public async Task<TenantGrowthAnalyticsDto> GetTenantGrowthAsync()
    {
        var tenants = await _platformDb.Tenants.AsNoTracking().ToListAsync();

        var now = DateTime.UtcNow;
        var startOfMonth = new DateTime(now.Year, now.Month, 1);
        var startOfYear = new DateTime(now.Year, 1, 1);
        var lastMonth = startOfMonth.AddMonths(-1);
        var lastYear = startOfYear.AddYears(-1);

        var newThisMonth = tenants.Count(t => t.CreatedAt >= startOfMonth);
        var newThisYear = tenants.Count(t => t.CreatedAt >= startOfYear);
        var newLastMonth = tenants.Count(t => t.CreatedAt >= lastMonth && t.CreatedAt < startOfMonth);
        var newLastYear = tenants.Count(t => t.CreatedAt >= lastYear && t.CreatedAt < startOfYear);

        var monthGrowth = newLastMonth > 0 ? ((double)(newThisMonth - newLastMonth) / newLastMonth) * 100 : (newThisMonth > 0 ? 100 : 0);
        var yearGrowth = newLastYear > 0 ? ((double)(newThisYear - newLastYear) / newLastYear) * 100 : (newThisYear > 0 ? 100 : 0);

        var growthHistory = tenants
            .GroupBy(t => new { t.CreatedAt.Year, t.CreatedAt.Month })
            .Select(g => new TenantGrowthPoint(new DateTime(g.Key.Year, g.Key.Month, 1), g.Count()))
            .OrderBy(g => g.Date)
            .ToList();

        return new TenantGrowthAnalyticsDto(newThisMonth, newThisYear, monthGrowth, yearGrowth, growthHistory);
    }

    public async Task<SubscriptionDistributionDto> GetSubscriptionDistributionAsync()
    {
        var tenants = await _platformDb.Tenants
            .AsNoTracking()
            .Include(t => t.SubscriptionPlan)
            .ToListAsync();

        var basicCount = tenants.Count(t => t.SubscriptionPlan.Category == PlanCategory.Basic);
        var proCount = tenants.Count(t => t.SubscriptionPlan.Category == PlanCategory.Professional);
        var enterpriseCount = tenants.Count(t => t.SubscriptionPlan.Category == PlanCategory.Enterprise);

        var statusDistribution = tenants
            .GroupBy(t => t.SubscriptionStatus)
            .ToDictionary(g => g.Key.ToString(), g => g.Count());

        return new SubscriptionDistributionDto(basicCount, proCount, enterpriseCount, statusDistribution);
    }
}
