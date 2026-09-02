using System.ComponentModel.DataAnnotations;
using Khet360.Domain.Enums;

namespace Khet360.Domain.Entities;

/// <summary>
/// Represents a recorded interaction with a Lead, Opportunity, or Customer.
/// </summary>
public class Activity : IBranchScoped
{
    [Key]
    public Guid Id { get; set; }

    // Polymorphic Links
    public Guid? LeadId { get; set; }
    public virtual Lead? Lead { get; set; }

    public Guid? OpportunityId { get; set; }
    public virtual Opportunity? Opportunity { get; set; }

    public Guid? CustomerId { get; set; }
    public virtual Customer? Customer { get; set; }

    [Required]
    public ActivityType Type { get; set; }

    [Required]
    public ActivityStatus Status { get; set; } = ActivityStatus.Pending;

    [Required, MaxLength(255)]
    public string Subject { get; set; } = null!;

    public string? Description { get; set; }

    [Required]
    public DateTime ScheduledDate { get; set; }

    public DateTime? CompletedDate { get; set; }

    public string? Outcome { get; set; }

    public Guid? WorkItemId { get; set; }
    public virtual WorkItem? WorkItem { get; set; }

    [Required]
    public Guid BranchId { get; set; }


    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
