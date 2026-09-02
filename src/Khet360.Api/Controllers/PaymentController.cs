using Khet360.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Khet360.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PaymentController : ControllerBase
{
    private readonly IPaymentService _paymentService;

    public PaymentController(IPaymentService paymentService)
    {
        _paymentService = paymentService;
    }

    [HttpPost("invoice")]
    public async Task<IActionResult> CreateInvoice([FromBody] CreateInvoiceRequest request)
    {
        var invoice = await _paymentService.CreateInvoiceAsync(request.FuneralCaseId, request.Amount, request.DueDate);
        return Ok(invoice);
    }

    [HttpGet("invoice/{id}/payment-link")]
    public async Task<IActionResult> GetPaymentLink(Guid id)
    {
        var link = await _paymentService.GeneratePaymentLinkAsync(id);
        return Ok(new { PaymentUrl = link });
    }

    [HttpGet("case/{caseId}/invoices")]
    public async Task<IActionResult> GetInvoices(Guid caseId)
    {
        var invoices = await _paymentService.GetInvoicesByCaseAsync(caseId);
        return Ok(invoices);
    }

    [HttpGet("invoice/{invoiceId}/payments")]
    public async Task<IActionResult> GetPayments(Guid invoiceId)
    {
        var payments = await _paymentService.GetPaymentsByInvoiceAsync(invoiceId);
        return Ok(payments);
    }

    [HttpPost("webhook")]
    public async Task<IActionResult> ProcessWebhook([FromBody] WebhookRequest request)
    {
        await _paymentService.ProcessWebhookAsync(request.InvoiceId, request.Amount, request.TransactionRef);
        return Ok();
    }
}

public record CreateInvoiceRequest(Guid FuneralCaseId, decimal Amount, DateTime DueDate);
public record WebhookRequest(Guid InvoiceId, decimal Amount, string TransactionRef);
