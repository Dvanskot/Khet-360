using System;
using System.Threading.Tasks;

namespace Khet360.Application.Interfaces;

public interface IPaymentService
{
    Task<Khet360.Domain.Entities.Invoice> CreateInvoiceAsync(Guid funeralCaseId, decimal amount, DateTime dueDate);
    Task<string> GeneratePaymentLinkAsync(Guid invoiceId);
    Task ProcessWebhookAsync(Guid invoiceId, decimal amount, string transactionRef);
}
