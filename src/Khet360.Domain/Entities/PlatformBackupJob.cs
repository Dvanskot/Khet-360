using System;
using System.ComponentModel.DataAnnotations;
using Khet360.Domain.Common;

namespace Khet360.Domain.Entities;

public enum BackupStatus
{
    Pending,
    InProgress,
    Completed,
    Failed
}

public class PlatformBackupJob : BaseEntity
{
    [Required]
    public DateTime RequestedAtUtc { get; set; } = DateTime.UtcNow;

    public DateTime? CompletedAtUtc { get; set; }

    [Required]
    public BackupStatus Status { get; set; } = BackupStatus.Pending;

    public string? BackupFileKey { get; set; } // Path in MinIO S3

    public string? ErrorMessage { get; set; }

    public long FileSize { get; set; }
}
