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

public record EMP201Report(
    Guid PayrollRunId,
    string Period,
    decimal TotalPaye,
    decimal TotalUif,
    decimal TotalSdl,
    decimal TotalLiability
);

public record IRP5Certificate(
    Guid EmployeeId,
    string EmployeeName,
    string TaxNumber,
    decimal TotalEarnings,
    decimal TotalPaye,
    decimal TotalUif
);

public interface ISARSReportingService
{
    Task<EMP201Report> GenerateEMP201Async(Guid payrollRunId);
    Task<List<IRP5Certificate>> GenerateAnnualIRP5sAsync(Guid taxYearId);
}

public class SARSReportingService : ISARSReportingService
{
    private readonly TenantDbContext _db;

    public SARSReportingService(TenantDbContext db)
    {
        _db = db;
    }

    public async Task<EMP201Report> GenerateEMP201Async(Guid payrollRunId)
    {
        var run = await _db.PayrollRuns.FindAsync(payrollRunId);
        if (run == null) throw new KeyNotFoundException("Payroll run not found.");

        var statutoryEntries = await _db.PayrollEntries
            .Where(pe => pe.PayrollRunId == payrollRunId && pe.IsStatutory)
            .Include(pe => pe.PayItem)
            .ToListAsync();

        var totalPaye = statutoryEntries.Where(pe => pe.PayItem.Code == "PAYE").Sum(pe => pe.Amount);
        var totalUif = statutoryEntries.Where(pe => pe.PayItem.Code.Contains("UIF")).Sum(pe => pe.Amount);
        var totalSdl = statutoryEntries.Where(pe => pe.PayItem.Code == "SDL_Employer").Sum(pe => pe.Amount);

        return new EMP201Report(
            payrollRunId,
            run.PeriodName,
            totalPaye,
            totalUif,
            totalSdl,
            totalPaye + totalUif + totalSdl
        );
    }

    public async Task<List<IRP5Certificate>> GenerateAnnualIRP5sAsync(Guid taxYearId)
    {
        var taxYear = await _db.Set<TaxYear>().FindAsync(taxYearId);
        if (taxYear == null) throw new KeyNotFoundException("Tax year not found.");

        var runs = await _db.PayrollRuns
            .Where(r => r.StartDate >= taxYear.StartDate && r.EndDate <= taxYear.EndDate)
            .ToListAsync();

        var runIds = runs.Select(r => r.Id).ToList();

        var entries = await _db.PayrollEntries
            .Include(pe => pe.PayItem)
            .Where(pe => runIds.Contains(pe.PayrollRunId))
            .ToListAsync();

        var employeeIds = entries.Select(e => e.EmployeeId).Distinct().ToList();
        var certificates = new List<IRP5Certificate>();

        foreach (var empId in employeeIds)
        {
            var emp = await _db.Employees.FindAsync(empId);
            var profile = await _db.PayProfiles.FirstOrDefaultAsync(p => p.EmployeeId == empId);

            var empEntries = entries.Where(e => e.EmployeeId == empId).ToList();
            var earnings = empEntries.Where(e => e.PayItem.Type == PayItemType.Earning && !e.IsEmployerContribution).Sum(e => e.Amount);
            var paye = empEntries.Where(e => e.PayItem.Code == "PAYE").Sum(e => e.Amount);
            var uif = empEntries.Where(e => e.PayItem.Code == "UIF_Employee").Sum(e => e.Amount);

            certificates.Add(new IRP5Certificate(
                empId,
                $"{emp?.FirstName} {emp?.LastName}",
                profile?.TaxNumber ?? "N/A",
                earnings,
                paye,
                uif
            ));
        }

        return certificates;
    }
}
