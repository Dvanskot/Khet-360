using System;
using System.Collections.Generic;
using Khet360.Domain.Enums;
using Khet360.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace Khet360.Domain.Entities;

public enum InvoiceStatus
{
    Draft,
    Sent,
    Paid,
    Overdue,
    Cancelled
}

public class Invoice : IBranchScoped
{
    [Key]
    public Guid Id { get; set; }
    public string InvoiceNumber { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public DateTime DueDate { get; set; }
    public InvoiceStatus Status { get; set; } = InvoiceStatus.Draft;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Guid FuneralCaseId { get; set; }
    public virtual FuneralCase FuneralCase { get; set; } = null!;

    public Guid TenantId { get; set; }
    public Guid BranchId { get; set; }

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
}
