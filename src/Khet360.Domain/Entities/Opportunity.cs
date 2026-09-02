using System.ComponentModel.DataAnnotations;

namespace Khet360.Domain.Entities;

/// <summary>
/// Represents a qualified sales deal linked to a customer.
/// </summary>
public class Opportunity : IBranchScoped
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    public Guid CustomerId { get; set; }
    public virtual Customer Customer { get; set; } = null!;

    [Required, MaxLength(255)]
    public string Name { get; set; } = null!;

    [Required]
    public Khet360.Domain.Enums.OpportunityStage Stage { get; set; } = Khet360.Domain.Enums.OpportunityStage.Discovery;

    [Required]
    public decimal EstimatedValue { get; set; }

    [Required]
    public DateTime ExpectedCloseDate { get; set; }

    [Required]
    public Guid BranchId { get; set; }


    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public string? Notes { get; set; }

    public virtual ICollection<Activity> Activities { get; set; } = new List<Activity>();
}
