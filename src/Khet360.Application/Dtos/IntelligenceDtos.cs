using System;
using System.Collections.Generic;

namespace Khet360.Application.Dtos;

public record PlatformHealthDto(
    bool IsHealthy,
    double AverageResponseTimeMs,
    long TotalActiveTenants,
    long TotalSlaBreachesLast24h,
    string StatusMessage
);

public record TenantGrowthDto(
    DateTime Date,
    int NewTenants,
    int TotalTenants,
    double GrowthRate
);

public record FeatureUsageDto(
    string FeatureName,
    long TotalInvocations,
    double AverageDailyUsage,
    double PercentageOfTenantsUsing
);
