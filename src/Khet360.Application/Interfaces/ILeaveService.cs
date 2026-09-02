using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Khet360.Application.Dtos;

namespace Khet360.Application.Interfaces;

public interface ILeaveService
{
    Task<List<LeaveTypeDto>> GetLeaveTypesAsync();
    Task<Guid> CreateLeaveTypeAsync(LeaveTypeCreateDto dto);

    Task<List<LeaveBalanceDto>> GetEmployeeBalancesAsync(Guid employeeId);
    Task AdjustBalanceAsync(Guid employeeId, Guid leaveTypeId, double adjustment);

    Task<Guid> ApplyForLeaveAsync(LeaveApplicationCreateDto dto);
    Task<LeaveApplicationDto> GetLeaveApplicationAsync(Guid id);
    Task<List<LeaveApplicationDto>> GetLeaveApplicationsByEmployeeAsync(Guid employeeId);
    Task<List<LeaveApplicationDto>> GetPendingApplicationsAsync();
    Task ProcessLeaveApplicationAsync(Guid id, LeaveApprovalDto approval);
    Task CancelLeaveApplicationAsync(Guid id);
}

public record LeaveTypeCreateDto(string Name, string Code, bool IsPaid, double AnnualAccrualRate);
