namespace Khet360.Application.Dtos;

public record ProductivityScorecardDto(
    long TotalLeadsConverted,
    double AverageCaseClosureTimeSeconds,
    long TotalSlaBreaches,
    double SlaComplianceRate,
    double LeadConversionRate
);
