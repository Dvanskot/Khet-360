using System;
using Khet360.Domain.Common;

namespace Khet360.Domain.Entities;

public class EmploymentContract : BaseEntity
{
    public Guid EmployeeId { get; set; }
    public virtual Employee Employee { get; set; } = null!;

    public ContractType Type { get; set; } = ContractType.FullTime;
    public DateTime StartDate { get; set; } = DateTime.UtcNow;
    public DateTime? EndDate { get; set; }

    public decimal Salary { get; set; }
    public PaymentFrequency Frequency { get; set; } = PaymentFrequency.Monthly;
    public string? TermsAndConditions { get; set; }
}

public enum ContractType
{
    FullTime,
    PartTime,
    Contract,
    Casual
}

public enum PaymentFrequency
{
    Weekly,
    BiWeekly,
    Monthly
}
