using Khet360.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Khet360.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OpportunitiesController : ControllerBase
{
    private readonly IOpportunityService _opportunityService;

    public OpportunitiesController(IOpportunityService opportunityService)
    {
        _opportunityService = opportunityService;
    }

    [HttpGet]
    public async Task<IActionResult> GetOpportunities([FromQuery] OpportunitySearchFilter filter)
    {
        var result = await _opportunityService.SearchOpportunitiesAsync(filter);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetOpportunity(Guid id)
    {
        var opp = await _opportunityService.GetOpportunityAsync(id);
        if (opp == null) return NotFound();
        return Ok(opp);
    }

    [HttpPost]
    public async Task<IActionResult> CreateOpportunity([FromBody] OpportunityCreateDto oppDto, [FromQuery] Guid customerId, [FromQuery] Guid branchId)
    {
        var id = await _opportunityService.CreateOpportunityAsync(oppDto, customerId, branchId);
        return CreatedAtAction(nameof(GetOpportunity), new { id }, new { id });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateOpportunity(Guid id, [FromBody] OpportunityUpdateDto oppDto)
    {
        await _opportunityService.UpdateOpportunityAsync(id, oppDto);
        return NoContent();
    }

    [HttpPost("{id}/close")]
    public async Task<IActionResult> CloseOpportunity(Guid id, [FromBody] CloseOpportunityRequest request)
    {
        await _opportunityService.CloseOpportunityAsync(id, request.Won, request.Notes);
        return NoContent();
    }
}

public record CloseOpportunityRequest(bool Won, string Notes);
