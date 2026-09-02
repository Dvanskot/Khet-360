using System;
using Khet360.Domain.Common;

namespace Khet360.Domain.Entities;

public class InstallationSignOff : BaseEntity
{
    public Guid InstallationJobId { get; set; }
    public virtual InstallationJob InstallationJob { get; set; } = null!;

    public string CustomerName { get; set; } = string.Empty;
    public string SignatureData { get; set; } = string.Empty; // Base64 or reference to image
    public DateTime SignedAtUtc { get; set; } = DateTime.UtcNow;
    public string Comments { get; set; } = string.Empty;
    public bool IsSatisfied { get; set; }
}
