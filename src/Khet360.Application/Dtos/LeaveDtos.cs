using System;
using System.Collections.Generic;

namespace Khet360.Application.Dtos;

public record LeaveTypeDto(Guid Id, string Name, string Code, bool IsPaid, double AnnualAccrualRate);
public record LeaveBalanceDto(Guid Id, Guid EmployeeId, Guid LeaveTypeId, string LeaveTypeName, double Balance);
public record LeaveApplicationDto(
    Guid Id,
    Guid EmployeeId,
    string EmployeeName,
    Guid LeaveTypeId,
    string LeaveTypeName,
    DateTime StartDate,
    DateTime EndDate,
    double TotalDays,
    string Reason,
    string Status,
    Guid? ApprovedBy,
    string? ApproverName,
    string? RejectionReason,
    DateTime CreatedAtUtc
);

public record LeaveApplicationCreateDto(
    Guid EmployeeId,
    Guid LeaveTypeId,
    DateTime StartDate,
    DateTime EndDate,
    string Reason
);

public record LeaveApprovalDto(bool Approved, string? RejectionReason);
