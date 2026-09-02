using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Khet360.Domain.Entities;
using Khet360.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Khet360.Infrastructure.Services;

public interface IFinancialService
{
    Task RecordTransactionAsync(FinancialTransaction transaction, List<FinancialEntry> entries);
    Task PostTransactionAsync(Guid transactionId);
    Task<bool> IsTransactionImmutableAsync(Guid transactionId);
}

public class FinancialService : IFinancialService
{
    private readonly TenantDbContext _db;

    public FinancialService(TenantDbContext db)
    {
        _db = db;
    }

    public async Task RecordTransactionAsync(FinancialTransaction transaction, List<FinancialEntry> entries)
    {
        _db.FinancialTransactions.Add(transaction);
        _db.FinancialEntries.AddRange(entries);
        await _db.SaveChangesAsync();
    }

    public async Task PostTransactionAsync(Guid transactionId)
    {
        var transaction = await _db.FinancialTransactions.FindAsync(transactionId);
        if (transaction == null) throw new KeyNotFoundException("Transaction not found.");

        transaction.IsPosted = true;
        await _db.SaveChangesAsync();
    }

    public async Task<bool> IsTransactionImmutableAsync(Guid transactionId)
    {
        var transaction = await _db.FinancialTransactions.FindAsync(transactionId);
        return transaction?.IsPosted ?? false;
    }
}
