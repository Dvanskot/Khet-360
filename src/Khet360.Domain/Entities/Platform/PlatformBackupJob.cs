using System.ComponentModel.DataAnnotations;
using Khet360.Domain.Common;

namespace Khet360.Domain.Entities.Platform;

public enum BackupStatus
{
    Pending,
    InProgress,
    Completed,
    Failed
}

public enum RestoreStatus
{
    Pending,
    InProgress,
    Completed,
    Failed
}

public class PlatformBackupJob : BaseEntity
{
    [Required]
    public Guid TenantId { get; set; }
    [Required]
    public DateTime RequestedAtUtc { get; set; } = DateTime.UtcNow;

    public DateTime? CompletedAtUtc { get; set; }

    [Required]
    public BackupStatus Status { get; set; } = BackupStatus.Pending;

    public string? BackupFileKey { get; set; }
    public string? ErrorMessage { get; set; }
    public long FileSize { get; set; }
    public DateTime? ClaimedAtUtc { get; set; }
    public string? ClaimedBy { get; set; }
}

public class PlatformRestoreJob : BaseEntity
{
    [Required]
    public Guid TenantId { get; set; }
    [Required]
    public Guid BackupJobId { get; set; }
    [Required]
    public DateTime RequestedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? CompletedAtUtc { get; set; }
    [Required]
    public RestoreStatus Status { get; set; } = RestoreStatus.Pending;
    public string? ErrorMessage { get; set; }
    public DateTime? ClaimedAtUtc { get; set; }
    public string? ClaimedBy { get; set; }
}
