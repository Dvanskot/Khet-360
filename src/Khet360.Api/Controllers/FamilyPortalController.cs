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

    public FamilyPortalController(IFamilyPortalService portalService)
    {
        _portalService = portalService;
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
    public async Task<IActionResult> UploadDocument([FromQuery] string token, IFormFile file)
    {
        if (file == null || file.Length == 0) return BadRequest("No file uploaded.");

        using var stream = file.OpenReadStream();
        var docId = await _portalService.UploadDocumentAsync(token, stream, file.FileName, file.ContentType);

        return Ok(new { DocumentId = docId, Message = "Document uploaded successfully." });
    }
}
