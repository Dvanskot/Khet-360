using System;
using Khet360.Domain.Common;

namespace Khet360.Domain.Entities;

public class PayrollEntry : BaseEntity
{
    public Guid PayrollRunId { get; set; }
    public virtual PayrollRun PayrollRun { get; set; } = null!;

    public Guid EmployeeId { get; set; }
    public virtual Employee Employee { get; set; } = null!;

    public Guid PayItemId { get; set; }
    public virtual PayItem PayItem { get; set; } = null!;

    public decimal Amount { get; set; }
    public double Quantity { get; set; } // e.g., hours of overtime

    public bool IsStatutory { get; set; }
    public bool IsEmployerContribution { get; set; }
}
