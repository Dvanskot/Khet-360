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
    public async Task<IActionResult> GetLeads([FromQuery] LeadSearchFilter filter)
    {
        var result = await _leadService.SearchLeadsAsync(filter);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetLead(Guid id)
    {
        var lead = await _leadService.GetLeadAsync(id);
        if (lead == null) return NotFound();
        return Ok(lead);
    }

    [HttpPost]
    public async Task<IActionResult> CreateLead([FromBody] LeadCreateDto leadDto, [FromQuery] Guid branchId)
    {
        var id = await _leadService.CreateLeadAsync(leadDto, branchId);
        return CreatedAtAction(nameof(GetLead), new { id }, new { id });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateLead(Guid id, [FromBody] LeadUpdateDto leadDto)
    {
        await _leadService.UpdateLeadAsync(id, leadDto);
        return NoContent();
    }

    [HttpPost("{id}/convert")]
    public async Task<IActionResult> ConvertLead(Guid id, [FromBody] LeadConversionDto conversionDto)
    {
        var resultId = await _leadService.ConvertLeadAsync(id, conversionDto);
        return Ok(new { id = resultId });
    }
}
