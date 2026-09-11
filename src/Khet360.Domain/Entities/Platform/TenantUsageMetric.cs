using System;
using Khet360.Domain.Common;

namespace Khet360.Domain.Entities.Platform;

public class TenantUsageMetric : BaseEntity
{
    public Guid TenantId { get; set; }
    public string TenantSlug { get; set; } = string.Empty;
    public MetricType Type { get; set; }
    public string MetricCode { get; set; } = string.Empty;
    public decimal UsedValue { get; set; }
    public decimal LimitValue { get; set; }
    public DateTime RecordedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime PeriodStart { get; set; }
    public DateTime PeriodEnd { get; set; }
}

public enum MetricType
{
    BranchCount,
    EmployeeCount,
    StorageUsedBytes,
    ApiCalls,
    CaseCount,
    InvoiceCount,
    Custom
}
