using System;

namespace Khet360.Domain.Entities;

public class ArrangementItem : IBranchScoped
{
    public Guid Id { get; set; }
    public string ItemName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    public bool IsProvidedByFamily { get; set; }

    public Guid ServiceArrangementId { get; set; }
    public virtual ServiceArrangement ServiceArrangement { get; set; } = null!;

    public Guid TenantId { get; set; }
    public Guid BranchId { get; set; }
}
