using System;
using System.Collections.Generic;
using Khet360.Domain.Common;

namespace Khet360.Domain.Entities;

public class Employee : BaseEntity
{
    public Guid? UserId { get; set; }
    public virtual User? User { get; set; }

    public Guid DepartmentId { get; set; }
    public virtual Department Department { get; set; } = null!;

    public Guid PositionId { get; set; }

    public Guid BranchId { get; set; }
    public virtual Branch Branch { get; set; } = null!;

    public Guid? ManagerId { get; set; }
    public virtual Employee? Manager { get; set; }

    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string EmployeeCode { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; } = DateTime.UtcNow;


    public EmployeeStatus Status { get; set; } = EmployeeStatus.Active;
    public DateTime HireDate { get; set; } = DateTime.UtcNow;
    public DateTime? TerminationDate { get; set; }

    public string? EmergencyContactName { get; set; }
    public string? EmergencyContactPhone { get; set; }
    public string? Qualifications { get; set; }

    public virtual EmploymentContract Contract { get; set; } = null!;
}

public enum EmployeeStatus
{
    Active,
    OnLeave,
    Terminated,
    Suspended
}
