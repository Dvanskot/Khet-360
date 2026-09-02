using System;
using Khet360.Domain.Common;

namespace Khet360.Domain.Entities;

public class LeaveBalance : BaseEntity
{
    public Guid EmployeeId { get; set; }
    public virtual Employee Employee { get; set; } = null!;

    public Guid LeaveTypeId { get; set; }
    public virtual LeaveType LeaveType { get; set; } = null!;

    public double Balance { get; set; }
    public DateTime LastUpdatedUtc { get; set; } = DateTime.UtcNow;
}
