using System.ComponentModel.DataAnnotations;

namespace Khet360.Domain.Entities;

/// <summary>
/// Audit trail for WorkItem state changes and ownership transfers.
/// </summary>
public class WorkItemHistory
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    public Guid WorkItemId { get; set; }
    public virtual WorkItem WorkItem { get; set; } = null!;

    public Guid? ChangedByUserId { get; set; }

    public string? OldStatus { get; set; }
    public string? NewStatus { get; set; }

    public Guid? OldOwnerId { get; set; }
    public Guid? NewOwnerId { get; set; }

    public string? Note { get; set; }

    public DateTime TimestampUtc { get; set; } = DateTime.UtcNow;
}
