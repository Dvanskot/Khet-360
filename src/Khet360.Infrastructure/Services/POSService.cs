using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Khet360.Application.Dtos;
using Khet360.Application.Interfaces;
using Khet360.Domain.Entities;
using Khet360.Domain.Enums;
using Khet360.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Khet360.Infrastructure.Services;

public class POSService : IPOSService
{
    private readonly TenantDbContext _db;
    private readonly IInventoryService _inventoryService;
    private readonly ITenantUserContext _userContext;

    public POSService(TenantDbContext db, IInventoryService inventoryService, ITenantUserContext userContext)
    {
        _db = db;
        _inventoryService = inventoryService;
        _userContext = userContext;
    }

    public async Task<Guid> CreateQuickSaleAsync(POSSaleRequest request)
    {
        var currentUserId = _userContext.UserId;
        var correlationId = Guid.NewGuid().ToString();

        // 1. Validate stock
        // 1. Validate stock
        foreach (var item in request.Items)
        {
            var stock = await _inventoryService.GetStockLevelAsync(item.ProductId, request.BranchId);
            if (stock < item.Quantity)
            {
                throw new InvalidOperationException($"Insufficient stock for product {item.ProductId}. Available: {stock}, Requested: {item.Quantity}");
            }
        }

        // 2. Create Invoice
        var invoice = new Invoice
        {
            Id = Guid.NewGuid(),
            FuneralCaseId = Guid.Empty, // POS sales might not have a case, but the entity requires it.
            // In a real scenario, we might create a dummy case or make it optional.
            // For now, using a "POS-Sale" marker if possible, but the current entity has [Required].
            // I'll create a dummy FuneralCase for the POS transaction.
        };

        var dummyCase = new FuneralCase
        {
            Id = Guid.NewGuid(),
            CaseNumber = $"POS-{Guid.NewGuid().ToString()[..8]}",
            Status = FuneralCaseStatus.Closed,
            CustomerId = request.CustomerId,
            OpenedAt = DateTime.UtcNow,
            ClosedAt = DateTime.UtcNow,
            BranchId = request.BranchId,
            Notes = "POS Quick Sale"
        };
        _db.FuneralCases.Add(dummyCase);
        invoice.FuneralCaseId = dummyCase.Id;
        _db.Invoices.Add(invoice);

        // 3. Record Payment
        var payment = new Payment
        {
            Id = Guid.NewGuid(),
            InvoiceId = invoice.Id,
            Amount = request.PaymentAmount,
            PaymentDate = DateTime.UtcNow,
            TransactionReference = request.PaymentReference
        };
        _db.Payments.Add(payment);

        // 4. Financial Ledger
        var transaction = new FinancialTransaction
        {
            Id = Guid.NewGuid(),
            Description = $"POS Sale: {invoice.Id}",
            TransactionDate = DateTime.UtcNow,
            SourceEntityId = invoice.Id,
            SourceEntityType = "Invoice"
        };
        _db.FinancialTransactions.Add(transaction);

        // Revenue: Debit Cash/Bank, Credit Sales Revenue
        _db.FinancialEntries.Add(new FinancialEntry
        {
            Id = Guid.NewGuid(),
            FinancialTransactionId = transaction.Id,
            AccountCode = "CASH-BANK",
            Debit = request.PaymentAmount,
            Credit = 0
        });

        _db.FinancialEntries.Add(new FinancialEntry
        {
            Id = Guid.NewGuid(),
            FinancialTransactionId = transaction.Id,
            AccountCode = "SALES-REVENUE",
            Debit = 0,
            Credit = request.PaymentAmount
        });

        // 5. Inventory Stock-Out
        foreach (var item in request.Items)
        {
            await _inventoryService.UpdateStockAsync(
                item.ProductId,
                request.BranchId,
                -item.Quantity,
                InventoryTransactionType.Sale,
                _userContext.UserId ?? throw new UnauthorizedAccessException("User identity not found."),
                correlationId,
                invoice.Id.ToString());
        }

        await _db.SaveChangesAsync();
        return invoice.Id;
    }
}
