using System;
using System.ComponentModel.DataAnnotations;

namespace Khet360.Domain.Entities.Tenant;

/// <summary>
/// Tracks the current quantity of a specific product available at a branch.
/// Implements IBranchScoped for isolation.
/// </summary>
public class InventoryStock : IBranchScoped
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    public Guid ProductId { get; set; }
    public virtual FuneralProduct Product { get; set; } = null!;

    [Required]
    public int QuantityOnHand { get; set; }

    [Required]
    public int ReorderLevel { get; set; } = 5;

    [Required]
    public Guid BranchId { get; set; }
}
