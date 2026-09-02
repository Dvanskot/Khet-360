using System;
using System.Collections.Generic;
using Khet360.Domain.Common;

namespace Khet360.Domain.Entities;

public class LeaveType : BaseEntity
{
    public string Name { get; set; } = string.Empty; // e.g., Annual, Sick, Maternity
    public string Code { get; set; } = string.Empty; // e.g., AL, SL, MAT
    public bool IsPaid { get; set; } = true;
    public double AnnualAccrualRate { get; set; } // Days per year
}
