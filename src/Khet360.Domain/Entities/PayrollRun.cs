using System;
using System.Collections.Generic;
using Khet360.Domain.Common;

namespace Khet360.Domain.Entities;

public class PayrollRun : BaseEntity
{
    public string PeriodName { get; set; } = string.Empty; // e.g., "August 2026"
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public PayrollRunStatus Status { get; set; } = PayrollRunStatus.Draft;
    public DateTime? FinalizedDate { get; set; }
    public Guid? ApprovedBy { get; set; }

    public virtual ICollection<PayrollEntry> Entries { get; set; } = new List<PayrollEntry>();
}

public enum PayrollRunStatus
{
    Draft,
    Approved,
    Finalized
}
