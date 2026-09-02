using System;
using Khet360.Domain.Common;

namespace Khet360.Domain.Entities;

public class LeaveApplication : BaseEntity
{
    public Guid EmployeeId { get; set; }
    public virtual Employee Employee { get; set; } = null!;

    public Guid LeaveTypeId { get; set; }

    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Reason { get; set; } = string.Empty;

    public LeaveStatus Status { get; set; } = LeaveStatus.Draft;
    public Guid? ApprovedBy { get; set; }
    public virtual Employee? Approver { get; set; }
    public string? RejectionReason { get; set; }

    public double TotalDays { get; set; }
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}

public enum LeaveStatus
{
    Draft,
    Submitted,
    Approved,
    Rejected,
    Cancelled
}
