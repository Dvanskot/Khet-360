using System.ComponentModel.DataAnnotations;
using Khet360.Domain.Enums;

namespace Khet360.Domain.Entities;

/// <summary>
/// Represents a potential customer (Lead) before they are qualified and converted.
/// </summary>
public class Lead : IBranchScoped
{
    [Key]
    public Guid Id { get; set; }

    [Required, MaxLength(100)]
    public string FirstName { get; set; } = null!;

    [Required, MaxLength(100)]
    public string LastName { get; set; } = null!;

    [Required, EmailAddress, MaxLength(255)]
    public string Email { get; set; } = null!;

    [Required, MaxLength(50)]
    public string Phone { get; set; } = null!;

    [MaxLength(100)]
    public string Source { get; set; } = "Unknown";

    [MaxLength(100)]
    public string? CompanyName { get; set; }

    [MaxLength(100)]
    public string? Industry { get; set; }

    [Required]
    public LeadStatus Status { get; set; } = LeadStatus.New;

    public string? Notes { get; set; }

    [Required]
    public Guid BranchId { get; set; }


    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ConvertedAt { get; set; }

    public virtual ICollection<Activity> Activities { get; set; } = new List<Activity>();
}
