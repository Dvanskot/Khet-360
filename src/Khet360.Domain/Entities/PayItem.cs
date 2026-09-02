using System;
using Khet360.Domain.Common;

namespace Khet360.Domain.Entities;

public class PayItem : BaseEntity
{
    public string Name { get; set; } = string.Empty; // e.g., Basic Salary, Pension, Overtime
    public string Code { get; set; } = string.Empty;
    public PayItemType Type { get; set; } = PayItemType.Earning;
    public bool IsStatutory { get; set; } // Tax, UI, etc.
}

public enum PayItemType
{
    Earning,
    Deduction
}
