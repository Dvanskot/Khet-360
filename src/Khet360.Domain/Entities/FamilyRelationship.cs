using System.ComponentModel.DataAnnotations;
using Khet360.Domain.Enums;

namespace Khet360.Domain.Entities;

/// <summary>
/// Represents a relationship between two customers.
/// Implements IBranchScoped to ensure relationship records are isolated per branch.
/// Uses temporal fields (ValidFrom, ValidTo) to provide a full audit trail of relationship changes.
/// </summary>
public class FamilyRelationship : IBranchScoped
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    public Guid FromCustomerId { get; set; }
    public virtual Customer FromCustomer { get; set; } = null!;

    [Required]
    public Guid ToCustomerId { get; set; }
    public virtual Customer ToCustomer { get; set; } = null!;

    [Required]
    public RelationshipType Type { get; set; }

    [Required]
    public DateTime ValidFrom { get; set; } = DateTime.UtcNow;

    public DateTime? ValidTo { get; set; }

    public bool IsActive { get; set; } = true;

    [Required]
    public Guid BranchId { get; set; }

    public Guid TenantId { get; set; }
}
