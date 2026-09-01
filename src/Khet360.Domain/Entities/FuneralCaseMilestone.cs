using System.ComponentModel.DataAnnotations;
using Khet360.Domain.Enums;

namespace Khet360.Domain.Entities;

/// <summary>
/// Records the completion of a specific milestone in a funeral case.
/// Provides a full audit trail of the case's progression.
/// </summary>
public class FuneralCaseMilestone
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    public Guid CaseId { get; set; }
    public virtual FuneralCase Case { get; set; } = null!;

    [Required]
    public FuneralCaseStatus MilestoneStatus { get; set; }

    [Required]
    public DateTime CompletedAt { get; set; } = DateTime.UtcNow;

    [Required]
    public Guid CompletedByUserId { get; set; }

    public string? Outcome { get; set; }

    public string? Notes { get; set; }
}
