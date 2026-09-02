using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Khet360.Application.Dtos;
using Khet360.Application.Interfaces;
using Khet360.Domain.Entities;
using Khet360.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Khet360.Infrastructure.Services;

public class LeaveService : ILeaveService
{
    private readonly TenantDbContext _db;
    private readonly PlatformDbContext _platformDb;

    public LeaveService(TenantDbContext db, PlatformDbContext platformDb)
    {
        _db = db;
        _platformDb = platformDb;
    }

    public async Task<List<LeaveTypeDto>> GetLeaveTypesAsync()
    {
        return await _platformDb.LeaveTypes
            .Select(lt => new LeaveTypeDto(lt.Id, lt.Name, lt.Code, lt.IsPaid, lt.AnnualAccrualRate))
            .ToListAsync();
    }

    public async Task<Guid> CreateLeaveTypeAsync(LeaveTypeCreateDto dto)
    {
        var lt = new LeaveType
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            Code = dto.Code,
            IsPaid = dto.IsPaid,
            AnnualAccrualRate = dto.AnnualAccrualRate
        };
        _platformDb.LeaveTypes.Add(lt);
        await _platformDb.SaveChangesAsync();
        return lt.Id;
    }

    public async Task<List<LeaveBalanceDto>> GetEmployeeBalancesAsync(Guid employeeId)
    {
        var balances = await _db.LeaveBalances
            .Where(lb => lb.EmployeeId == employeeId)
            .ToListAsync();

        var leaveTypes = await _platformDb.LeaveTypes.ToListAsync();
        var typeMap = leaveTypes.ToDictionary(lt => lt.Id, lt => lt.Name);

        return balances.Select(lb => new LeaveBalanceDto(
            lb.Id,
            lb.EmployeeId,
            lb.LeaveTypeId,
            typeMap.GetValueOrDefault(lb.LeaveTypeId, "Unknown"),
            lb.Balance)).ToList();
    }

    public async Task AdjustBalanceAsync(Guid employeeId, Guid leaveTypeId, double adjustment)
    {
        var balance = await _db.LeaveBalances
            .FirstOrDefaultAsync(lb => lb.EmployeeId == employeeId && lb.LeaveTypeId == leaveTypeId);

        if (balance == null)
        {
            balance = new LeaveBalance
            {
                Id = Guid.NewGuid(),
                EmployeeId = employeeId,
                LeaveTypeId = leaveTypeId,
                Balance = adjustment
            };
            _db.LeaveBalances.Add(balance);
        }
        else
        {
            balance.Balance += adjustment;
            balance.LastUpdatedUtc = DateTime.UtcNow;
        }

        await _db.SaveChangesAsync();
    }

    public async Task<Guid> ApplyForLeaveAsync(LeaveApplicationCreateDto dto)
    {
        var totalDays = (dto.EndDate - dto.StartDate).TotalDays + 1;

        var application = new LeaveApplication
        {
            Id = Guid.NewGuid(),
            EmployeeId = dto.EmployeeId,
            LeaveTypeId = dto.LeaveTypeId,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            Reason = dto.Reason,
            Status = LeaveStatus.Submitted,
            TotalDays = totalDays,
            CreatedAtUtc = DateTime.UtcNow
        };

        _db.LeaveApplications.Add(application);
        await _db.SaveChangesAsync();
        return application.Id;
    }

    public async Task<LeaveApplicationDto> GetLeaveApplicationAsync(Guid id)
    {
        var app = await _db.LeaveApplications
            .Include(la => la.Employee)
            .Include(la => la.Approver)
            .FirstOrDefaultAsync(la => la.Id == id);

        if (app == null) throw new KeyNotFoundException("Leave application not found.");

        var leaveType = await _platformDb.LeaveTypes.FindAsync(app.LeaveTypeId);
        return MapToDto(app, leaveType?.Name ?? "Unknown");
    }

    public async Task<List<LeaveApplicationDto>> GetLeaveApplicationsByEmployeeAsync(Guid employeeId)
    {
        var apps = await _db.LeaveApplications
            .Where(la => la.EmployeeId == employeeId)
            .Include(la => la.Employee)
            .Include(la => la.Approver)
            .ToListAsync();

        var leaveTypes = await _platformDb.LeaveTypes.ToListAsync();
        var typeMap = leaveTypes.ToDictionary(lt => lt.Id, lt => lt.Name);

        return apps.Select(la => MapToDto(la, typeMap.GetValueOrDefault(la.LeaveTypeId, "Unknown"))).ToList();
    }

    public async Task<List<LeaveApplicationDto>> GetPendingApplicationsAsync()
    {
        var apps = await _db.LeaveApplications
            .Where(la => la.Status == LeaveStatus.Submitted)
            .Include(la => la.Employee)
            .Include(la => la.Approver)
            .ToListAsync();

        var leaveTypes = await _platformDb.LeaveTypes.ToListAsync();
        var typeMap = leaveTypes.ToDictionary(lt => lt.Id, lt => lt.Name);

        return apps.Select(la => MapToDto(la, typeMap.GetValueOrDefault(la.LeaveTypeId, "Unknown"))).ToList();
    }

    public async Task ProcessLeaveApplicationAsync(Guid id, LeaveApprovalDto approval)
    {
        var app = await _db.LeaveApplications.FindAsync(id);
        if (app == null) throw new KeyNotFoundException("Leave application not found.");

        if (approval.Approved)
        {
            app.Status = LeaveStatus.Approved;
            // In a real system, we would deduct the balance here
            var balance = await _db.LeaveBalances
                .FirstOrDefaultAsync(lb => lb.EmployeeId == app.EmployeeId && lb.LeaveTypeId == app.LeaveTypeId);

            if (balance != null)
            {
                balance.Balance -= app.TotalDays;
                balance.LastUpdatedUtc = DateTime.UtcNow;
            }
        }
        else
        {
            app.Status = LeaveStatus.Rejected;
            app.RejectionReason = approval.RejectionReason;
        }

        // Note: ApproverId should be set from the current user context
        // For now, we'll assume the caller handles the identity
        await _db.SaveChangesAsync();
    }

    public async Task CancelLeaveApplicationAsync(Guid id)
    {
        var app = await _db.LeaveApplications.FindAsync(id);
        if (app == null) throw new KeyNotFoundException("Leave application not found.");

        if (app.Status == LeaveStatus.Approved)
        {
            // Refund balance
            var balance = await _db.LeaveBalances
                .FirstOrDefaultAsync(lb => lb.EmployeeId == app.EmployeeId && lb.LeaveTypeId == app.LeaveTypeId);
            if (balance != null)
            {
                balance.Balance += app.TotalDays;
                balance.LastUpdatedUtc = DateTime.UtcNow;
            }
        }

        app.Status = LeaveStatus.Cancelled;
        await _db.SaveChangesAsync();
    }

    private static LeaveApplicationDto MapToDto(LeaveApplication la, string leaveTypeName)
    {
        return new LeaveApplicationDto(
            la.Id,
            la.EmployeeId,
            la.Employee?.FirstName + " " + la.Employee?.LastName,
            la.LeaveTypeId,
            leaveTypeName,
            la.StartDate,
            la.EndDate,
            la.TotalDays,
            la.Reason,
            la.Status.ToString(),
            la.ApprovedBy,
            la.Approver?.FirstName + " " + la.Approver?.LastName,
            la.RejectionReason,
            la.CreatedAtUtc
        );
    }
}
