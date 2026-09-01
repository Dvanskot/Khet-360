using System;
using System.Collections.Generic;
using Khet360.Domain.Enums;

namespace Khet360.Domain.Entities;

public class Vendor : IBranchScoped
{
    public Guid Id { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public string ContactName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty; // e.g., "Florist", "Caterer", "Hearse Provider"
    public VendorStatus Status { get; set; } = VendorStatus.Pending;
    public string? TaxId { get; set; }
    public string? BankDetails { get; set; }

    public Guid TenantId { get; set; }
    public Guid BranchId { get; set; }
}

public class VendorOrder : IBranchScoped
{
    public Guid Id { get; set; }
    public string OrderReference { get; set; } = string.Empty;
    public VendorOrderStatus Status { get; set; } = VendorOrderStatus.Requested;
    public DateTime OrderedAt { get; set; }
    public DateTime? ConfirmedAt { get; set; }
    public DateTime? DeliveredAt { get; set; }
    public decimal TotalAmount { get; set; }
    public string? Notes { get; set; }

    public Guid VendorId { get; set; }
    public virtual Vendor Vendor { get; set; } = null!;

    public Guid FuneralCaseId { get; set; }
    public virtual FuneralCase FuneralCase { get; set; } = null!;

    public Guid TenantId { get; set; }
    public Guid BranchId { get; set; }

    public virtual ICollection<VendorOrderItem> Items { get; set; } = new List<VendorOrderItem>();
}

public class VendorOrderItem : IBranchScoped
{
    public Guid Id { get; set; }
    public string ItemDescription { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public bool IsConfirmed { get; set; }

    public Guid VendorOrderId { get; set; }
    public virtual VendorOrder VendorOrder { get; set; } = null!;

    public Guid TenantId { get; set; }
    public Guid BranchId { get; set; }
}
