using System;
using System.ComponentModel.DataAnnotations;
using Khet360.Domain.Enums;

namespace Khet360.Domain.Entities;

public enum InventoryTransactionType
{
    Purchase,
    Sale,
    Adjustment,
    Wastage
}

/// <summary>
/// Audit log of all stock movements for products across branches.
/// Implements IBranchScoped for isolation.
/// </summary>
public class InventoryTransaction : IBranchScoped
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    public Guid ProductId { get; set; }
    public virtual FuneralProduct Product { get; set; } = null!;

    [Required]
    public int QuantityChanged { get; set; } // Positive for stock-in, negative for stock-out

    [Required]
    public InventoryTransactionType TransactionType { get; set; }

    public string? ReferenceId { get; set; } // e.g., InvoiceId, VendorOrderId

    public string? Notes { get; set; }

    [Required]
    public Guid UserId { get; set; }

    public string? CorrelationId { get; set; }

    [Required]
    public DateTime TimestampUtc { get; set; } = DateTime.UtcNow;

    [Required]
    public Guid BranchId { get; set; }
}
