using Khet360.Application.Dtos;
using Khet360.Application.Interfaces;
using Khet360.Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Khet360.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LeadsController : ControllerBase
{
    private readonly ILeadService _leadService;

    public LeadsController(ILeadService leadService)
    {
        _leadService = leadService;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedList<LeadDto>>>> GetLeads([FromQuery] LeadSearchFilter filter)
    {
        var result = await _leadService.SearchLeadsAsync(filter);
        return Ok(ApiResponse.Ok(result, "Leads retrieved successfully"));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<LeadDto>>> GetLead(Guid id)
    {
        var lead = await _leadService.GetLeadAsync(id);
        if (lead == null) 
            return NotFound(ApiResponse.Fail<LeadDto>("Lead not found"));
        
        return Ok(ApiResponse.Ok(lead, "Lead retrieved successfully"));
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<Guid>>> CreateLead([FromBody] LeadCreateDto leadDto, [FromQuery] Guid branchId)
    {
        var id = await _leadService.CreateLeadAsync(leadDto, branchId);
        return CreatedAtAction(nameof(GetLead), new { id }, ApiResponse.Ok(id, "Lead created successfully"));
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<object>>> UpdateLead(Guid id, [FromBody] LeadUpdateDto leadDto)
    {
        await _leadService.UpdateLeadAsync(id, leadDto);
        return Ok(ApiResponse.Ok(new { success = true }, "Lead updated successfully"));
    }

    [HttpPost("{id}/convert")]
    public async Task<ActionResult<ApiResponse<Guid>>> ConvertLead(Guid id, [FromBody] LeadConversionDto conversionDto)
    {
        var resultId = await _leadService.ConvertLeadAsync(id, conversionDto);
        return Ok(ApiResponse.Ok(resultId, "Lead converted successfully"));
    }
}
