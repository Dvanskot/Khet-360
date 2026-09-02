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

public class PayrollService : IPayrollService
{
    private readonly TenantDbContext _db;

    public PayrollService(TenantDbContext db)
    {
        _db = db;
    }

    public async Task<PayProfileDto> GetPayProfileAsync(Guid employeeId)
    {
        var profile = await _db.PayProfiles.FirstOrDefaultAsync(p => p.EmployeeId == employeeId);
        if (profile == null) throw new KeyNotFoundException("Pay profile not found.");
        return new PayProfileDto(profile.Id, profile.EmployeeId, profile.BankName, profile.AccountNumber, profile.BranchCode, profile.TaxNumber, profile.TaxBracket);
    }

    public async Task<Guid> CreatePayProfileAsync(PayProfileCreateDto dto)
    {
        var profile = new PayProfile
        {
            Id = Guid.NewGuid(),
            EmployeeId = dto.EmployeeId,
            BankName = dto.BankName,
            AccountNumber = dto.AccountNumber,
            BranchCode = dto.BranchCode,
            TaxNumber = dto.TaxNumber,
            TaxBracket = dto.TaxBracket
        };
        _db.PayProfiles.Add(profile);
        await _db.SaveChangesAsync();
        return profile.Id;
    }

    public async Task<List<PayItemDto>> GetPayItemsAsync()
    {
        return await _db.PayItems
            .Select(pi => new PayItemDto(pi.Id, pi.Name, pi.Code, pi.Type.ToString(), pi.IsStatutory))
            .ToListAsync();
    }

    public async Task<Guid> CreatePayItemAsync(PayItemCreateDto dto)
    {
        var item = new PayItem
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            Code = dto.Code,
            Type = Enum.Parse<PayItemType>(dto.Type),
            IsStatutory = dto.IsStatutory
        };
        _db.PayItems.Add(item);
        await _db.SaveChangesAsync();
        return item.Id;
    }

    public async Task<Guid> CreatePayrollRunAsync(PayrollRunCreateDto dto)
    {
        var run = new PayrollRun
        {
            Id = Guid.NewGuid(),
            PeriodName = dto.PeriodName,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            Status = PayrollRunStatus.Draft
        };
        _db.PayrollRuns.Add(run);
        await _db.SaveChangesAsync();
        return run.Id;
    }

    public async Task<PayrollRunDto> GetPayrollRunAsync(Guid id)
    {
        var run = await _db.PayrollRuns.FindAsync(id);
        if (run == null) throw new KeyNotFoundException("Payroll run not found.");
        return new PayrollRunDto(run.Id, run.PeriodName, run.StartDate, run.EndDate, run.Status.ToString(), run.FinalizedDate, run.ApprovedBy);
    }

    public async Task CalculatePayrollAsync(Guid runId)
    {
        var run = await _db.PayrollRuns.FindAsync(runId);
        if (run == null) throw new KeyNotFoundException("Payroll run not found.");
        if (run.Status != PayrollRunStatus.Draft) throw new InvalidOperationException("Only draft runs can be calculated.");

        var employees = await _db.Employees
            .Include(e => e.Contract)
            .ToListAsync();

        var payItems = await _db.PayItems.ToListAsync();
        var basicSalaryItem = payItems.FirstOrDefault(pi => pi.Code == "BASIC");

        if (basicSalaryItem == null) throw new InvalidOperationException("Basic salary pay item (CODE: BASIC) must be defined.");

        // Clear existing entries for this run
        var existingEntries = await _db.PayrollEntries.Where(pe => pe.PayrollRunId == runId).ToListAsync();
        _db.PayrollEntries.RemoveRange(existingEntries);

        foreach (var emp in employees)
        {
            if (emp.Contract == null) continue;

            // 1. Base Salary
            var entry = new PayrollEntry
            {
                Id = Guid.NewGuid(),
                PayrollRunId = runId,
                EmployeeId = emp.Id,
                PayItemId = basicSalaryItem.Id,
                Amount = emp.Contract.Salary,
                Quantity = 1
            };
            _db.PayrollEntries.Add(entry);
        }

        await _db.SaveChangesAsync();
    }

    public async Task FinalizePayrollRunAsync(Guid runId, Guid approvedBy)
    {
        var run = await _db.PayrollRuns.FindAsync(runId);
        if (run == null) throw new KeyNotFoundException("Payroll run not found.");

        run.Status = PayrollRunStatus.Finalized;
        run.FinalizedDate = DateTime.UtcNow;
        run.ApprovedBy = approvedBy;

        // Generate Payslips
        var entries = await _db.PayrollEntries
            .Where(pe => pe.PayrollRunId == runId)
            .ToListAsync();

        var employeeGroups = entries.GroupBy(pe => pe.EmployeeId);

        foreach (var group in employeeGroups)
        {
            var employeeId = group.Key;
            var grossPay = group.Where(pe => pe.PayItem.Type == PayItemType.Earning).Sum(pe => pe.Amount);
            var totalDeductions = group.Where(pe => pe.PayItem.Type == PayItemType.Deduction).Sum(pe => pe.Amount);

            var payslip = new Payslip
            {
                Id = Guid.NewGuid(),
                EmployeeId = employeeId,
                PayrollRunId = runId,
                GrossPay = grossPay,
                TotalDeductions = totalDeductions,
                NetPay = grossPay - totalDeductions,
                IssuedDate = DateTime.UtcNow
            };
            _db.Payslips.Add(payslip);
        }

        // Note: In a real app, we would post to the Finance General Ledger here.

        await _db.SaveChangesAsync();
    }

    public async Task<PayslipDto> GetPayslipAsync(Guid employeeId, Guid runId)
    {
        var payslip = await _db.Payslips
            .FirstOrDefaultAsync(ps => ps.EmployeeId == employeeId && ps.PayrollRunId == runId);

        if (payslip == null) throw new KeyNotFoundException("Payslip not found.");

        var employee = await _db.Employees.FindAsync(employeeId);
        var run = await _db.PayrollRuns.FindAsync(runId);
        var entries = await _db.PayrollEntries
            .Where(pe => pe.PayrollRunId == runId && pe.EmployeeId == employeeId)
            .Include(pe => pe.PayItem)
            .ToListAsync();

        return new PayslipDto(
            payslip.Id,
            payslip.EmployeeId,
            employee?.FirstName + " " + employee?.LastName,
            payslip.PayrollRunId,
            run?.PeriodName ?? "Unknown",
            payslip.GrossPay,
            payslip.TotalDeductions,
            payslip.NetPay,
            payslip.IssuedDate,
            entries.Select(pe => new PayrollEntryDto(
                pe.Id, pe.PayrollRunId, pe.EmployeeId, employee?.FirstName + " " + employee?.LastName,
                pe.PayItemId, pe.PayItem.Name, pe.Amount, pe.Quantity
            )).ToList()
        );
    }
}
