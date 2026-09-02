using System;
using Khet360.Domain.Common;

namespace Khet360.Domain.Entities;

public class Payslip : BaseEntity
{
    public Guid EmployeeId { get; set; }
    public virtual Employee Employee { get; set; } = null!;

    public Guid PayrollRunId { get; set; }
    public virtual PayrollRun PayrollRun { get; set; } = null!;

    public decimal GrossPay { get; set; }
    public decimal TotalDeductions { get; set; }
    public decimal NetPay { get; set; }
    public DateTime IssuedDate { get; set; } = DateTime.UtcNow;
}
