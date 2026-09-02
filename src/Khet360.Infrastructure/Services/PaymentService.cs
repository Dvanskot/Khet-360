using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Khet360.Application.Interfaces;
using Khet360.Domain.Entities;
using Khet360.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Khet360.Infrastructure.Services;

public class PaymentService : IPaymentService
{
    private readonly TenantDbContext _db;
    private readonly ITenantService _tenantService;

    public PaymentService(TenantDbContext db, ITenantService tenantService)
    {
        _db = db;
        _tenantService = tenantService;
    }

    public async Task<Invoice> CreateInvoiceAsync(Guid funeralCaseId, decimal amount, DateTime dueDate)
    {
        var tenantId = _tenantService.CurrentTenant?.Id
            ?? throw new InvalidOperationException("Tenant context not found.");

        var funeralCase = await _db.FuneralCases.FindAsync(funeralCaseId);
        if (funeralCase == null) throw new KeyNotFoundException("Funeral Case not found.");

        var invoice = new Invoice
        {
            Id = Guid.NewGuid(),
            InvoiceNumber = $"INV-{Guid.NewGuid().ToString()[..8].ToUpper()}",
            TotalAmount = amount,
            DueDate = dueDate,
            Status = InvoiceStatus.Sent,
            FuneralCaseId = funeralCaseId,
            TenantId = tenantId,
            BranchId = funeralCase.BranchId
        };

        _db.Invoices.Add(invoice);
        await _db.SaveChangesAsync();

        return invoice;
    }

    public async Task<string> GeneratePaymentLinkAsync(Guid invoiceId)
    {
        var invoice = await _db.Invoices.FindAsync(invoiceId);
        if (invoice == null) throw new KeyNotFoundException("Invoice not found.");

        // Mocking Netcash integration. In production, this would call the Netcash API
        // using a secret key and invoice details to get a hosted payment page URL.
        return $"https://pay.netcash.co.za/pay/{invoice.InvoiceNumber}?amount={invoice.TotalAmount}";
    }

    public async Task ProcessWebhookAsync(Guid invoiceId, decimal amount, string transactionRef)
    {
        var invoice = await _db.Invoices.FindAsync(invoiceId);
        if (invoice == null) throw new KeyNotFoundException("Invoice not found.");

        var payment = new Payment
        {
            Id = Guid.NewGuid(),
            Amount = amount,
            PaymentDate = DateTime.UtcNow,
            TransactionReference = transactionRef,
            PaymentMethod = "Netcash",
            InvoiceId = invoiceId,
            TenantId = invoice.TenantId,
            BranchId = invoice.BranchId
        };

        _db.Payments.Add(payment);

        var totalPaid = await _db.Payments
            .Where(p => p.InvoiceId == invoiceId)
            .SumAsync(p => p.Amount);

        if (totalPaid >= invoice.TotalAmount)
        {
            invoice.Status = InvoiceStatus.Paid;
        }

        await _db.SaveChangesAsync();
    }

    public async Task<List<Invoice>> GetInvoicesByCaseAsync(Guid funeralCaseId)
    {
        return await _db.Invoices
            .Where(i => i.FuneralCaseId == funeralCaseId)
            .ToListAsync();
    }

    public async Task<List<Payment>> GetPaymentsByInvoiceAsync(Guid invoiceId)
    {
        return await _db.Payments
            .Where(p => p.InvoiceId == invoiceId)
            .ToListAsync();
    }
}
