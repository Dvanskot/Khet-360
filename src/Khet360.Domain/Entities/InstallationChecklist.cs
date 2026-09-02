using System;
using Khet360.Domain.Common;

namespace Khet360.Domain.Entities;

public class InstallationChecklist : BaseEntity
{
    public Guid InstallationJobId { get; set; }
    public virtual InstallationJob InstallationJob { get; set; } = null!;

    public string Requirement { get; set; } = string.Empty; // e.g., "Plot verified", "Base ready"
    public bool IsVerified { get; set; }
    public DateTime? VerifiedAtUtc { get; set; }
    public Guid? VerifiedBy { get; set; }
}
