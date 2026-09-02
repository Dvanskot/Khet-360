using System;
using Khet360.Domain.Common;
using Khet360.Domain.Enums;

namespace Khet360.Domain.Entities;

public class PaymentTransaction : BaseEntity
{
    public Guid InvoiceId { get; set; }
    public Invoice Invoice { get; set; } = null!;
    public string TransactionReference { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public TransactionStatus Status { get; set; }
    public string? GatewayResponse { get; set; }
    public string? ErrorMessage { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public Guid BranchId { get; set; }
}
