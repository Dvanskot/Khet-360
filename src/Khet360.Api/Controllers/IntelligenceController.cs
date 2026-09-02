using Khet360.Application.Dtos;
using Khet360.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Khet360.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class IntelligenceController : ControllerBase
{
    private readonly IIntelligenceService _intelligenceService;

    public IntelligenceController(IIntelligenceService intelligenceService)
    {
        _intelligenceService = intelligenceService;
    }

    [HttpGet("health")]
    public async Task<IActionResult> GetHealth()
    {
        var health = await _intelligenceService.GetPlatformHealthAsync();
        return Ok(health);
    }

    [HttpGet("growth")]
    public async Task<IActionResult> GetGrowth([FromQuery] DateTime from, [FromQuery] DateTime to)
    {
        var growth = await _intelligenceService.GetTenantGrowthTrendsAsync(from, to);
        return Ok(growth);
    }

    [HttpGet("usage")]
    public async Task<IActionResult> GetUsage()
    {
        var usage = await _intelligenceService.GetGlobalFeatureUsageAsync();
        return Ok(usage);
    }
}
