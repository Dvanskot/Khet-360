namespace Khet360.Application.Dtos;

using Khet360.Domain.Entities.Tenant;
using Khet360.Domain.Entities.Platform;
using Khet360.Domain.Entities.Common;

public record ProductivityScorecardDto(
    long TotalLeadsConverted,
    double AverageCaseClosureTimeSeconds,
    long TotalSlaBreaches,
    double SlaComplianceRate,
    double LeadConversionRate
);
