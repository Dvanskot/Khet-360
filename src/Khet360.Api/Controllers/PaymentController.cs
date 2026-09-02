using Khet360.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Khet360.Infrastructure.Services;
using Khet360.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Khet360.Infrastructure.Persistence;

namespace Khet360.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PaymentController : ControllerBase
{
    private readonly IPaymentService _paymentService;
    private readonly IWebhookValidator _webhookValidator;
    private readonly TenantDbContext _db;

    public PaymentController(IPaymentService paymentService, IWebhookValidator webhookValidator, TenantDbContext db)
    {
        _paymentService = paymentService;
        _webhookValidator = webhookValidator;
        _db = db;
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
    public async Task<IActionResult> ProcessWebhook([FromBody] WebhookRequest request, [FromHeader(Name = "X-Payment-Signature")] string signature)
    {
        var config = await _db.PaymentConfigurations.FirstOrDefaultAsync();
        if (config == null) return BadRequest("Payment gateway not configured.");

        // Read request body for signature validation
        // In a real production app, we'd use a custom middleware or a request filter to capture the raw body
        // For this implementation, we assume the payload is reconstructed from the request object
        var payload = System.Text.Json.JsonSerializer.Serialize(request);

        if (!await _webhookValidator.ValidateSignatureAsync(config, payload, signature))
        {
            return Unauthorized("Invalid webhook signature.");
        }

        await _paymentService.ProcessWebhookAsync(request.InvoiceId, request.Amount, request.TransactionRef);
        return Ok();
    }
}

public record CreateInvoiceRequest(Guid FuneralCaseId, decimal Amount, DateTime DueDate);
public record WebhookRequest(Guid InvoiceId, decimal Amount, string TransactionRef);
