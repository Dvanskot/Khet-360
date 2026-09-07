using Khet360.Application.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Khet360.Application.Interfaces;
using Khet360.Api.Attributes;
using Khet360.Domain.Enums;
using System;

namespace Khet360.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class FuneralCasesController : ControllerBase
{
    private readonly IFuneralCaseService _funeralCaseService;

    public FuneralCasesController(IFuneralCaseService funeralCaseService)
    {
        _funeralCaseService = funeralCaseService;
    }

    [HttpPost]
    [HasPermission("FuneralCases.Create")]
    public async Task<ActionResult<ApiResponse<Guid>>> OpenCase([FromBody] OpenCaseRequest request)
    {
        var id = await _funeralCaseService.OpenCaseAsync(request.CustomerId, request.DeceasedCustomerId, request.BranchId);
        return CreatedAtAction(nameof(GetById), new { id }, ApiResponse.Ok(id, "Case opened successfully"));
    }

    [HttpPatch("milestone")]
    [HasPermission("FuneralCases.Update")]
    public async Task<ActionResult<ApiResponse<object>>> CompleteMilestone([FromBody] CompleteMilestoneRequest request)
    {
        var userId = Guid.Parse(User.FindFirst("sub")?.Value ?? throw new UnauthorizedAccessException());
        await _funeralCaseService.CompleteMilestoneAsync(request.CaseId, request.Milestone, request.Outcome, request.Notes, userId);
        return Ok(ApiResponse.Ok(new { success = true }, "Milestone completed successfully"));
    }

    [HttpGet("{id}")]
    [HasPermission("FuneralCases.Read")]
    public async Task<ActionResult<ApiResponse<FuneralCaseDetailsDto>>> GetById(Guid id)
    {
        var funeralCase = await _funeralCaseService.GetCaseDetailsAsync(id);
        if (funeralCase == null) 
            return NotFound(ApiResponse.Fail<FuneralCaseDetailsDto>("Case not found"));
        
        return Ok(ApiResponse.Ok(funeralCase, "Case retrieved successfully"));
    }

    [HttpGet]
    [HasPermission("FuneralCases.Read")]
    public async Task<ActionResult<ApiResponse<PagedList<FuneralCaseDto>>>> GetCases([FromQuery] FuneralCaseSearchFilter filter)
    {
        var results = await _funeralCaseService.SearchCasesAsync(filter);
        return Ok(ApiResponse.Ok(results, "Cases retrieved successfully"));
    }
}

public record OpenCaseRequest(Guid CustomerId, Guid? DeceasedCustomerId, Guid BranchId);
public record CompleteMilestoneRequest(Guid CaseId, FuneralCaseStatus Milestone, string Outcome, string Notes);
