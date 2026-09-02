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
    private readonly ITaxService _taxService;
    private readonly IFinancialService _financialService;

    public PayrollService(TenantDbContext db, ITaxService taxService, IFinancialService financialService)
    {
        _db = db;
        _taxService = taxService;
        _financialService = financialService;
    }

    public async Task<PayProfileDto> GetPayProfileAsync(Guid employeeId)
    {
        var profile = await _db.PayProfiles.FirstOrDefaultAsync(p => p.EmployeeId == employeeId);
        if (profile == null) throw new KeyNotFoundException("Pay profile not found.");
        return new PayProfileDto(profile.Id, profile.EmployeeId, profile.BankName, profile.AccountNumber, profile.BranchCode, profile.TaxNumber, profile.TaxBracket);
    }

    private async Task<PayItem> GetOrCreateStatutoryItemAsync(string code, string name, PayItemType type)
    {
        var item = await _db.PayItems.FirstOrDefaultAsync(pi => pi.Code == code);
        if (item != null) return item;

        item = new PayItem
        {
            Id = Guid.NewGuid(),
            Name = name,
            Code = code,
            Type = type,
            IsStatutory = true
        };
        _db.PayItems.Add(item);
        await _db.SaveChangesAsync();
        return item;
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

        // Get statutory items
        var payeItem = await GetOrCreateStatutoryItemAsync("PAYE", "PAYE Tax", PayItemType.Deduction);
        var uifEmployeeItem = await GetOrCreateStatutoryItemAsync("UIF_Employee", "UIF Employee", PayItemType.Deduction);
        var uifEmployerItem = await GetOrCreateStatutoryItemAsync("UIF_Employer", "UIF Employer", PayItemType.Earning);
        var sdlEmployerItem = await GetOrCreateStatutoryItemAsync("SDL_Employer", "SDL Employer", PayItemType.Earning);

        // Get active tax year
        var taxYearId = await _taxService.GetActiveTaxYearIdAsync();

        // Clear existing entries for this run
        var existingEntries = await _db.PayrollEntries.Where(pe => pe.PayrollRunId == runId).ToListAsync();
        _db.PayrollEntries.RemoveRange(existingEntries);

        foreach (var emp in employees)
        {
            if (emp.Contract == null) continue;

            var profile = await _db.PayProfiles.FirstOrDefaultAsync(p => p.EmployeeId == emp.Id);
            if (profile == null) throw new InvalidOperationException($"Employee {emp.EmployeeCode} has no pay profile configured.");

            decimal grossPay = emp.Contract.Salary;

            // 1. Base Salary
            _db.PayrollEntries.Add(new PayrollEntry
            {
                Id = Guid.NewGuid(),
                PayrollRunId = runId,
                EmployeeId = emp.Id,
                PayItemId = basicSalaryItem.Id,
                Amount = grossPay,
                Quantity = 1
            });

            // 2. PAYE Calculation
            var paye = await _taxService.CalculatePayeAsync(grossPay, emp.DateOfBirth, taxYearId);
            if (paye > 0)
            {
                _db.PayrollEntries.Add(new PayrollEntry
                {
                    Id = Guid.NewGuid(),
                    PayrollRunId = runId,
                    EmployeeId = emp.Id,
                    PayItemId = payeItem.Id,
                    Amount = paye,
                    Quantity = 1,
                    IsStatutory = true
                });
            }

            // 3. UIF and SDL
            var statutory = await _taxService.CalculateStatutoryDeductionsAsync(grossPay, taxYearId);

            if (statutory.EmployeeUif > 0)
            {
                _db.PayrollEntries.Add(new PayrollEntry
                {
                    Id = Guid.NewGuid(),
                    PayrollRunId = runId,
                    EmployeeId = emp.Id,
                    PayItemId = uifEmployeeItem.Id,
                    Amount = statutory.EmployeeUif,
                    Quantity = 1,
                    IsStatutory = true
                });
            }

            if (statutory.EmployerUif > 0)
            {
                _db.PayrollEntries.Add(new PayrollEntry
                {
                    Id = Guid.NewGuid(),
                    PayrollRunId = runId,
                    EmployeeId = emp.Id,
                    PayItemId = uifEmployerItem.Id,
                    Amount = statutory.EmployerUif,
                    Quantity = 1,
                    IsStatutory = true,
                    IsEmployerContribution = true
                });
            }

            if (statutory.EmployerSdl > 0)
            {
                _db.PayrollEntries.Add(new PayrollEntry
                {
                    Id = Guid.NewGuid(),
                    PayrollRunId = runId,
                    EmployeeId = emp.Id,
                    PayItemId = sdlEmployerItem.Id,
                    Amount = statutory.EmployerSdl,
                    Quantity = 1,
                    IsStatutory = true,
                    IsEmployerContribution = true
                });
            }
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
            .Include(pe => pe.PayItem)
            .Where(pe => pe.PayrollRunId == runId)
            .ToListAsync();

        var employeeGroups = entries.GroupBy(pe => pe.EmployeeId);

        decimal totalNetPay = 0;
        decimal totalStatutoryLiability = 0;
        decimal totalEmployerCost = 0;

        foreach (var group in employeeGroups)
        {
            var employeeId = group.Key;
            var grossPay = group.Where(pe => pe.PayItem.Type == PayItemType.Earning && !pe.IsEmployerContribution).Sum(pe => pe.Amount);
            var totalDeductions = group.Where(pe => pe.PayItem.Type == PayItemType.Deduction).Sum(pe => pe.Amount);
            var netPay = grossPay - totalDeductions;

            var payslip = new Payslip
            {
                Id = Guid.NewGuid(),
                EmployeeId = employeeId,
                PayrollRunId = runId,
                GrossPay = grossPay,
                TotalDeductions = totalDeductions,
                NetPay = netPay,
                IssuedDate = DateTime.UtcNow
            };
            _db.Payslips.Add(payslip);

            totalNetPay += netPay;
        }

        // Calculate Liabilities for SARS
        var statutoryEntries = entries.Where(pe => pe.IsStatutory).ToList();
        totalStatutoryLiability = statutoryEntries.Sum(pe => pe.Amount);
        totalEmployerCost = entries.Where(pe => pe.IsEmployerContribution).Sum(pe => pe.Amount);

        // Post to Financial Ledger
        var transaction = new FinancialTransaction
        {
            Id = Guid.NewGuid(),
            Description = $"Payroll Finalization: {run.PeriodName}",
            TransactionDate = DateTime.UtcNow,
            SourceEntityId = runId,
            SourceEntityType = "PayrollRun"
        };
        _db.FinancialTransactions.Add(transaction);

        // Debit: Payroll Expense (Gross Pay + Employer Contributions)
        var totalGross = entries.Where(pe => pe.PayItem.Type == PayItemType.Earning).Sum(pe => pe.Amount);
        _db.FinancialEntries.Add(new FinancialEntry
        {
            Id = Guid.NewGuid(),
            FinancialTransactionId = transaction.Id,
            AccountCode = "PAYROLL-EXP",
            Debit = totalGross,
            Credit = 0
        });

        // Credit: Bank (Net Pay)
        _db.FinancialEntries.Add(new FinancialEntry
        {
            Id = Guid.NewGuid(),
            FinancialTransactionId = transaction.Id,
            AccountCode = "CASH-BANK",
            Debit = 0,
            Credit = totalNetPay
        });

        // Credit: SARS Liability (PAYE + UIF + SDL)
        if (totalStatutoryLiability > 0)
        {
            _db.FinancialEntries.Add(new FinancialEntry
            {
                Id = Guid.NewGuid(),
                FinancialTransactionId = transaction.Id,
                AccountCode = "SARS-LIABILITY",
                Debit = 0,
                Credit = totalStatutoryLiability
            });
        }

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
