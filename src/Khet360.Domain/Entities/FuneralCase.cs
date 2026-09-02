using System.ComponentModel.DataAnnotations;
using Khet360.Domain.Enums;

namespace Khet360.Domain.Entities;

/// <summary>
/// Represents a funeral service case.
/// Implements IBranchScoped to ensure cases are isolated to the branch that manages them.
/// </summary>
public class FuneralCase : IBranchScoped
{
    [Key]
    public Guid Id { get; set; }

    [Required, MaxLength(50)]
    public string CaseNumber { get; set; } = null!;

    [Required]
    public FuneralCaseStatus Status { get; set; } = FuneralCaseStatus.Enquiry;

    [Required]
    public Guid CustomerId { get; set; }
    public virtual Customer Customer { get; set; } = null!;

    public Guid? DeceasedCustomerId { get; set; }
    public virtual Customer? DeceasedCustomer { get; set; }

    [Required]
    public DateTime OpenedAt { get; set; } = DateTime.UtcNow;

    public DateTime? ClosedAt { get; set; }
    public DateTime? ScheduledDate { get; set; }

    public string? Notes { get; set; }

    [Required]
    public Guid BranchId { get; set; }


    public virtual ICollection<FuneralCaseMilestone> Milestones { get; set; } = new List<FuneralCaseMilestone>();
}
