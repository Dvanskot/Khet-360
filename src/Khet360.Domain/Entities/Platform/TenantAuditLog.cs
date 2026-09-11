using System;
using Khet360.Domain.Common;

namespace Khet360.Domain.Entities.Platform;

public class TenantAuditLog : BaseEntity
{
    public Guid TenantId { get; set; }
    public string TenantSlug { get; set; } = string.Empty;
    public Guid? PerformedByUserId { get; set; }
    public string PerformedByUsername { get; set; } = string.Empty;
    public AuditAction Action { get; set; }
    public string EntityType { get; set; } = string.Empty;
    public string EntityId { get; set; } = string.Empty;
    public string? OldValues { get; set; }
    public string? NewValues { get; set; }
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    public DateTime PerformedAtUtc { get; set; } = DateTime.UtcNow;
    public bool IsSuccess { get; set; }
    public string? ErrorMessage { get; set; }
    public string? Reason { get; set; }
}

public enum AuditAction
{
    Created,
    Updated,
    Deleted,
    Suspended,
    Reactivated,
    Canceled,
    Provisioned,
    BackedUp,
    Migrated,
    PaymentProcessed,
    FeatureToggled,
    ConfigChanged
}
