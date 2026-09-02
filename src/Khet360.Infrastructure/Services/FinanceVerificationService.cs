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

public interface IFinanceVerificationService
{
    Task<FinanceVerificationResult> VerifyInvariantsAsync();
}

public record FinanceVerificationResult(
    bool IsBalanced,
    List<FinancialInvariantViolation> Violations
);

public record FinancialInvariantViolation(
    Guid TransactionId,
    string Description,
    decimal Variance
);

public class FinanceVerificationService : IFinanceVerificationService
{
    private readonly TenantDbContext _db;

    public FinanceVerificationService(TenantDbContext db)
    {
        _db = db;
    }

    public async Task<FinanceVerificationResult> VerifyInvariantsAsync()
    {
        var violations = new List<FinancialInvariantViolation>();

        var transactions = await _db.FinancialTransactions
            .Include(ft => ft.Entries)
            .ToListAsync();

        foreach (var tx in transactions)
        {
            var totalDebits = tx.Entries.Sum(e => e.Debit);
            var totalCredits = tx.Entries.Sum(e => e.Credit);
            var variance = totalDebits - totalCredits;

            if (variance != 0)
            {
                violations.Add(new FinancialInvariantViolation(
                    tx.Id,
                    $"Transaction unbalanced: Debits({totalDebits}) != Credits({totalCredits})",
                    variance
                ));
            }
        }

        return new FinanceVerificationResult(violations.Count == 0, violations);
    }
}
