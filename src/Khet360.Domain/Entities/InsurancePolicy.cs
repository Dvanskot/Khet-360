using System;
using Khet360.Domain.Enums;

namespace Khet360.Domain.Entities;

public class InsurancePolicy : IBranchScoped
{
    public Guid Id { get; set; }
    public string PolicyNumber { get; set; } = string.Empty;
    public string ProviderName { get; set; } = string.Empty;
    public decimal CoverageAmount { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public PolicyStatus Status { get; set; }

    public Guid CustomerId { get; set; }
    public Guid TenantId { get; set; }
    public Guid BranchId { get; set; }

    public virtual ICollection<InsuranceClaim> Claims { get; set; } = new List<InsuranceClaim>();
}
