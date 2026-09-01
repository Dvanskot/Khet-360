using System.ComponentModel.DataAnnotations;
using Khet360.Domain.Enums;

namespace Khet360.Domain.Entities;

/// <summary>
/// The atomic unit of "owned work" in Khet-360.
/// Wraps a business entity that requires action, ensuring visibility in My Work and Team Queues.
/// </summary>
public class WorkItem : IBranchScoped
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    public string SourceEntityType { get; set; } = null!; // e.g., "Lead", "Opportunity", "Activity"

    [Required]
    public Guid SourceEntityId { get; set; }

    public Guid? OwnerId { get; set; } // Null means it's in the Team Queue

    public Guid? TeamId { get; set; }

    [Required]
    public WorkItemPriority Priority { get; set; } = WorkItemPriority.Medium;

    [Required]
    public DateTime DueDate { get; set; }

    [Required]
    public WorkItemStatus Status { get; set; } = WorkItemStatus.Pending;

    public string? CurrentState { get; set; }

    public string? NextAction { get; set; }

    public SlaStatus SlaStatus { get; set; } = SlaStatus.OnTrack;

    [Required]
    public Guid BranchId { get; set; }

    public Guid TenantId { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<WorkItemHistory> History { get; set; } = new List<WorkItemHistory>();
}
