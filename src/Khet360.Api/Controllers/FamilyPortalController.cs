using Microsoft.AspNetCore.Mvc;
using Khet360.Application.Interfaces;
using Khet360.Application.Dtos;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Khet360.Api.Controllers;

[ApiController]
[Route("api/family-portal")]
public class FamilyPortalController : ControllerBase
{
    private readonly IFamilyPortalService _portalService;
    private readonly IPaymentService _paymentService;

    public FamilyPortalController(IFamilyPortalService portalService, IPaymentService paymentService)
    {
        _portalService = portalService;
        _paymentService = paymentService;
    }

    [HttpPost("generate-token")]
    public async Task<IActionResult> GenerateToken([FromBody] Guid caseId)
    {
        var response = await _portalService.GenerateCaseAccessTokenAsync(caseId);
        return Ok(response);
    }

    [HttpGet("case-view")]
    public async Task<IActionResult> GetCaseView([FromQuery] string token)
    {
        var view = await _portalService.GetCaseViewByTokenAsync(token);
        if (view == null) return Unauthorized("Invalid or expired access token.");
        return Ok(view);
    }

    [HttpPost("upload-document")]
    public async Task<IActionResult> UploadDocument([FromQuery] string token, [FromQuery] Guid documentRequestId, IFormFile file)
    {
        if (file == null || file.Length == 0) return BadRequest("No file uploaded.");

        using var stream = file.OpenReadStream();
        var docId = await _portalService.UploadDocumentAsync(token, stream, file.FileName, file.ContentType, documentRequestId);

        return Ok(new { DocumentId = docId, Message = "Document uploaded successfully." });
    }

    [HttpGet("payment-link/{invoiceId}")]
    public async Task<IActionResult> GetPaymentLink([FromQuery] string token, Guid invoiceId)
    {
        var view = await _portalService.GetCaseViewByTokenAsync(token);
        if (view == null) return Unauthorized("Invalid or expired access token.");

        var link = await _paymentService.GeneratePaymentLinkAsync(invoiceId);
        return Ok(new { PaymentUrl = link });
    }
}
