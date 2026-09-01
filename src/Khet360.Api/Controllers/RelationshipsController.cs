using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Khet360.Application.Interfaces;
using Khet360.Api.Attributes;
using Khet360.Domain.Enums;

namespace Khet360.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class RelationshipsController : ControllerBase
{
    private readonly IFamilyRelationshipService _relationshipService;

    public RelationshipsController(IFamilyRelationshipService relationshipService)
    {
        _relationshipService = relationshipService;
    }

    [HttpPost]
    [HasPermission("Relationships.Manage")]
    public async Task<IActionResult> CreateRelationship([FromBody] CreateRelationshipRequest request)
    {
        await _relationshipService.AddRelationshipAsync(request.FromCustomerId, request.ToCustomerId, request.Type);
        return Ok();
    }

    [HttpPatch("{id}")]
    [HasPermission("Relationships.Manage")]
    public async Task<IActionResult> TerminateRelationship(Guid id, [FromBody] TerminateRelationshipRequest request)
    {
        await _relationshipService.TerminateRelationshipAsync(id, request.EffectiveDate);
        return NoContent();
    }

    [HttpGet("customer/{customerId}")]
    [HasPermission("Relationships.Read")]
    public async Task<IActionResult> GetByCustomer(Guid customerId)
    {
        var relationships = await _relationshipService.GetRelationshipsForCustomerAsync(customerId);
        return Ok(relationships);
    }

    [HttpGet("graph/{customerId}")]
    [HasPermission("Relationships.Read")]
    public async Task<IActionResult> GetGraph(Guid customerId)
    {
        var graph = await _relationshipService.GetFamilyGraphAsync(customerId);
        return Ok(graph);
    }
}

public record CreateRelationshipRequest(Guid FromCustomerId, Guid ToCustomerId, RelationshipType Type);
public record TerminateRelationshipRequest(DateTime EffectiveDate);
