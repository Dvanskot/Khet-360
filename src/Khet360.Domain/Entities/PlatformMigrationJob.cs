using System;
using Khet360.Domain.Common;
using Khet360.Domain.Enums;

namespace Khet360.Domain.Entities;

public class PlatformMigrationJob : BaseEntity
{
    public Guid TenantId { get; set; }
    public IsolationTier SourceTier { get; set; }
    public IsolationTier TargetTier { get; set; }
    public DateTime RequestedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? CompletedAtUtc { get; set; }
    public MigrationStatus Status { get; set; } = MigrationStatus.Pending;
    public string? ErrorMessage { get; set; }
    public Guid? BackupJobId { get; set; }
}
