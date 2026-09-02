using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Khet360.Application.Interfaces;
using Khet360.Domain.Entities;
using Khet360.Domain.Enums;
using Khet360.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Khet360.Infrastructure.Services;

public class PaymentService : IPaymentService
{
    private readonly TenantDbContext _db;
    private readonly ITenantService _tenantService;
    private readonly IEnumerable<IPaymentGatewayProvider> _gatewayProviders;

    public PaymentService(TenantDbContext db, ITenantService tenantService, IEnumerable<IPaymentGatewayProvider> gatewayProviders)
    {
        _db = db;
        _tenantService = tenantService;
        _gatewayProviders = gatewayProviders;
    }

    public async Task<Invoice> CreateInvoiceAsync(Guid funeralCaseId, decimal amount, DateTime dueDate)
    {
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

        // Resolve the tenant's specific payment configuration
        var config = await _db.PaymentConfigurations.FirstOrDefaultAsync();
        if (config == null)
        {
            throw new InvalidOperationException("Payment gateway not configured for this tenant. Please set up your gateway in the administration portal.");
        }

        // Find the provider implementation that matches the config
        var provider = _gatewayProviders.FirstOrDefault(p =>
            p.ProviderName.Equals(config.Provider.ToString(), StringComparison.OrdinalIgnoreCase));

        if (provider == null)
        {
            throw new NotSupportedException($"Payment provider {config.Provider} is not currently supported.");
        }

        return await provider.CreatePaymentLinkAsync(config, invoice.TotalAmount, invoice.InvoiceNumber);
    }

    public async Task ProcessWebhookAsync(Guid invoiceId, decimal amount, string transactionRef)
    {
        var invoice = await _db.Invoices.FindAsync(invoiceId);
        if (invoice == null) throw new KeyNotFoundException("Invoice not found.");

        var config = await _db.PaymentConfigurations.FirstOrDefaultAsync();
        if (config == null) throw new InvalidOperationException("Payment gateway not configured.");

        var provider = _gatewayProviders.FirstOrDefault(p =>
            p.ProviderName.Equals(config.Provider.ToString(), StringComparison.OrdinalIgnoreCase));

        if (provider == null) throw new NotSupportedException($"Payment provider {config.Provider} not supported.");

        // Verify the payment with the provider
        var isValid = await provider.VerifyPaymentAsync(config, transactionRef, amount);
        if (!isValid) throw new InvalidOperationException("Payment verification failed.");

        var payment = new Payment
        {
            Id = Guid.NewGuid(),
            Amount = amount,
            PaymentDate = DateTime.UtcNow,
            TransactionReference = transactionRef,
            PaymentMethod = config.Provider.ToString(),
            InvoiceId = invoiceId,
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
