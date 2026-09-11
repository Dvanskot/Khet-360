using System;
using Khet360.Domain.Entities.Platform;

namespace Khet360.Application.Dtos;

public record PlatformOverviewDto(
    bool IsHealthy,
    int TotalActiveTenants,
    int TotalTrialingTenants,
    int TotalSuspendedTenants,
    int TotalCanceledTenants,
    decimal MonthlyRecurringRevenue,
    decimal AnnualRecurringRevenue,
    DateTime MeasuredAt
);

public record RevenueAnalyticsDto(
    decimal MonthlyRevenue,
    decimal AnnualRevenue,
    decimal AverageRevenuePerTenant,
    List<RevenueByPlanDto> RevenueByPlan
);

public record RevenueByPlanDto(string PlanName, decimal Revenue, int TenantCount);

public record TenantGrowthAnalyticsDto(
    int NewTenantsThisMonth,
    int NewTenantsThisYear,
    double MonthOverMonthGrowth,
    double YearOverYearGrowth,
    List<TenantGrowthPoint> GrowthHistory
);

public record TenantGrowthPoint(DateTime Date, int Count);

public record SubscriptionDistributionDto(
    int BasicCount,
    int ProfessionalCount,
    int EnterpriseCount,
    Dictionary<string, int> StatusDistribution
);

public record PlatformMetricDto(string Name, decimal Value, string Unit, DateTime RecordedAt);

public record AuditLogDto(
    Guid Id,
    Guid TenantId,
    string TenantSlug,
    string PerformedByUsername,
    AuditAction Action,
    string EntityType,
    string EntityId,
    bool IsSuccess,
    string? ErrorMessage,
    DateTime PerformedAtUtc,
    string? Reason = null
);

public record UsageMetricDto(
    Guid Id,
    Guid TenantId,
    string TenantSlug,
    MetricType Type,
    string MetricCode,
    decimal UsedValue,
    decimal LimitValue,
    DateTime RecordedAtUtc
);

public record FeatureFlagDto(
    Guid Id,
    string Name,
    string Code,
    string Description,
    FeatureFlagType Type,
    bool IsEnabledGlobally,
    bool IsEnabledForTenant,
    DateTime CreatedAt
);

public record AnnouncementDto(
    Guid Id,
    string Title,
    string Content,
    AnnouncementPriority Priority,
    AnnouncementScope Scope,
    bool IsActive,
    DateTime StartDate,
    DateTime? EndDate,
    DateTime CreatedAt
);
