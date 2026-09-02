using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Khet360.Domain.Entities;
using Khet360.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Khet360.Application.Interfaces;

namespace Khet360.Infrastructure.Services;

public record TaxCalculationResult(
    decimal MonthlyPaye,
    decimal EmployeeUif,
    decimal EmployerUif,
    decimal EmployerSdl
);

public interface ITaxService
{
    Task<Guid> GetActiveTaxYearIdAsync();
    Task<decimal> CalculatePayeAsync(decimal monthlyGross, DateTime birthDate, Guid taxYearId);
    Task<TaxCalculationResult> CalculateStatutoryDeductionsAsync(decimal monthlyGross, Guid taxYearId);
}

public class TaxService : ITaxService
{
    private readonly PlatformDbContext _platformDb;

    public TaxService(PlatformDbContext platformDb)
    {
        _platformDb = platformDb;
    }

    public async Task<Guid> GetActiveTaxYearIdAsync()
    {
        var taxYear = await _platformDb.TaxYears
            .FirstOrDefaultAsync(ty => ty.IsActive);

        if (taxYear == null) throw new InvalidOperationException("No active tax year configured in the system.");

        return taxYear.Id;
    }

    public async Task<decimal> CalculatePayeAsync(decimal monthlyGross, DateTime birthDate, Guid taxYearId)
    {
        var annualTaxableIncome = monthlyGross * 12;

        var bracket = await _platformDb.TaxBrackets
            .Where(b => b.TaxYearId == taxYearId && annualTaxableIncome >= b.LowerLimit)
            .OrderByDescending(b => b.LowerLimit)
            .FirstOrDefaultAsync();

        if (bracket == null) return 0;

        // Base tax + % of amount over lower limit
        var grossAnnualTax = bracket.BaseAmount + ((annualTaxableIncome - bracket.LowerLimit) * bracket.PercentageOverLowerLimit);

        // Apply Rebates
        var age = CalculateAge(birthDate);
        var rebates = await _platformDb.TaxRebates
            .Where(r => r.TaxYearId == taxYearId && age >= r.MinAge)
            .ToListAsync();

        var totalRebates = rebates.Sum(r => r.Amount);
        var netAnnualTax = Math.Max(0, grossAnnualTax - totalRebates);

        return netAnnualTax / 12;
    }

    public async Task<TaxCalculationResult> CalculateStatutoryDeductionsAsync(decimal monthlyGross, Guid taxYearId)
    {
        var rates = await _platformDb.StatutoryRates
            .Where(r => r.TaxYearId == taxYearId)
            .ToListAsync();

        var uifEmployeeRate = rates.FirstOrDefault(r => r.RateName == "UIF_Employee")?.Percentage ?? 0;
        var uifEmployerRate = rates.FirstOrDefault(r => r.RateName == "UIF_Employer")?.Percentage ?? 0;
        var sdlEmployerRate = rates.FirstOrDefault(r => r.RateName == "SDL_Employer")?.Percentage ?? 0;
        var uifCappingLimit = rates.FirstOrDefault(r => r.RateName == "UIF_CappingLimit")?.CappingLimit ?? decimal.MaxValue;

        // UIF is calculated on the lower of gross pay or the cap
        var uifBase = Math.Min(monthlyGross, uifCappingLimit);
        var employeeUif = uifBase * uifEmployeeRate;
        var employerUif = uifBase * uifEmployerRate;
        var employerSdl = monthlyGross * sdlEmployerRate;

        return new TaxCalculationResult(0, employeeUif, employerUif, employerSdl);
    }

    private int CalculateAge(DateTime birthDate)
    {
        var today = DateTime.UtcNow;
        var age = today.Year - birthDate.Year;
        if (birthDate.Date > today.AddYears(-age)) age--;
        return age;
    }
}
