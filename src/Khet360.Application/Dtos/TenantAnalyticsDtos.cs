using System;
using System.Collections.Generic;

namespace Khet360.Application.Dtos;

public record OperationalEfficiencyDto(
    TimeSpan AvgLeadToOpportunityTime,
    TimeSpan AvgOpportunityToCaseTime,
    TimeSpan AvgCaseCompletionTime,
    double EfficiencyScore
);

public record BranchPerformanceDto(
    Guid BranchId,
    string BranchName,
    decimal TotalRevenue,
    int TotalCasesCompleted,
    decimal AverageCaseValue
);

public record SlaComplianceDto(
    double OverallComplianceRate,
    int TotalBreaches,
    List<SlaBreachDetail> TopBreachReasons
);

public record SlaBreachDetail(
    string Reason,
    int Count,
    double Percentage
);

public record WorkloadDistributionDto(
    int TotalActiveWorkItems,
    double AvgItemsPerUser,
    List<UserWorkloadDto> UserWorkloads
);

public record UserWorkloadDto(
    Guid UserId,
    string UserName,
    int ActiveItems,
    int CompletedItems
);
