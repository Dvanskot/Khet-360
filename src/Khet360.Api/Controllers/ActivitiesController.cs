using Khet360.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Khet360.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ActivitiesController : ControllerBase
{
    private readonly IActivityService _activityService;

    public ActivitiesController(IActivityService activityService)
    {
        _activityService = activityService;
    }

    [HttpGet]
    public async Task<IActionResult> GetActivities([FromQuery] ActivitySearchFilter filter)
    {
        var result = await _activityService.SearchActivitiesAsync(filter);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetActivity(Guid id)
    {
        var activity = await _activityService.GetActivityAsync(id);
        if (activity == null) return NotFound();
        return Ok(activity);
    }

    [HttpPost]
    public async Task<IActionResult> CreateActivity([FromBody] ActivityCreateDto activityDto, [FromQuery] Guid branchId)
    {
        var id = await _activityService.CreateActivityAsync(activityDto, branchId);
        return CreatedAtAction(nameof(GetActivity), new { id }, new { id });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateActivity(Guid id, [FromBody] ActivityUpdateDto activityDto)
    {
        await _activityService.UpdateActivityAsync(id, activityDto);
        return NoContent();
    }

    [HttpPost("{id}/complete")]
    public async Task<IActionResult> CompleteActivity(Guid id, [FromBody] CompleteActivityRequest request)
    {
        await _activityService.CompleteActivityAsync(id, request.Outcome, request.Notes);
        return NoContent();
    }
}

public record CompleteActivityRequest(string Outcome, string Notes);
