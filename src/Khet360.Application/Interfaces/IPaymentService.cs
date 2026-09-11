using System;
using Khet360.Domain.Entities.Common;
using Khet360.Domain.Entities.Platform;
using System.Threading.Tasks;
using Khet360.Domain.Entities.Tenant;

namespace Khet360.Application.Interfaces;

public interface IPaymentService
{
    Task<Invoice> CreateInvoiceAsync(Guid funeralCaseId, decimal amount, DateTime dueDate);
    Task<string> GeneratePaymentLinkAsync(Guid invoiceId);
    Task ProcessWebhookAsync(Guid invoiceId, decimal amount, string transactionRef);
    Task<List<Invoice>> GetInvoicesByCaseAsync(Guid funeralCaseId);
    Task<List<Payment>> GetPaymentsByInvoiceAsync(Guid invoiceId);
}
