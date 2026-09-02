using System;
using Khet360.Domain.Enums;

namespace Khet360.Domain.Entities;

public class InsuranceClaim : IBranchScoped
{
    public Guid Id { get; set; }
    public string ClaimNumber { get; set; } = string.Empty;
    public decimal ClaimAmount { get; set; }
    public ClaimStatus Status { get; set; }
    public DateTime SubmittedAt { get; set; }
    public DateTime? ProcessedAt { get; set; }
    public string? Notes { get; set; }

    public Guid PolicyId { get; set; }
    public virtual InsurancePolicy Policy { get; set; } = null!;

    public Guid FuneralCaseId { get; set; }
    public virtual FuneralCase FuneralCase { get; set; } = null!;

    public Guid BranchId { get; set; }

    public virtual ICollection<ClaimPayment> Payments { get; set; } = new List<ClaimPayment>();
}
