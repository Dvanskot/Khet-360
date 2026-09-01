using System;

namespace Khet360.Domain.Entities;

public class ClaimPayment : IBranchScoped
{
    public Guid Id { get; set; }
    public decimal Amount { get; set; }
    public DateTime PaymentDate { get; set; }
    public string TransactionReference { get; set; } = string.Empty;
    public string? Notes { get; set; }

    public Guid ClaimId { get; set; }
    public virtual InsuranceClaim Claim { get; set; } = null!;

    public Guid TenantId { get; set; }
    public Guid BranchId { get; set; }
}
