using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Khet360.Application.Interfaces;
using Khet360.Application.Dtos;

namespace Khet360.Api.Controllers.Platform;

[Authorize(AuthenticationSchemes = "PlatformJwt", Roles = "PlatformAdmin")]
[ApiController]
[Route("api/platform/analytics")]
public class PlatformAnalyticsController : ControllerBase
{
    private readonly IPlatformAnalyticsService _analyticsService;
    private readonly ILogger<PlatformAnalyticsController> _logger;

    public PlatformAnalyticsController(IPlatformAnalyticsService analyticsService, ILogger<PlatformAnalyticsController> logger)
    {
        _analyticsService = analyticsService;
        _logger = logger;
    }

    [HttpGet("health")]
    public async Task<ActionResult<PlatformOverviewDto>> GetHealth()
    {
        var health = await _analyticsService.GetPlatformHealthAsync();
        return Ok(health);
    }

    [HttpGet("revenue")]
    public async Task<ActionResult<RevenueAnalyticsDto>> GetRevenue()
    {
        var revenue = await _analyticsService.GetRevenueAnalyticsAsync();
        return Ok(revenue);
    }

    [HttpGet("growth")]
    public async Task<ActionResult<TenantGrowthAnalyticsDto>> GetGrowth()
    {
        var growth = await _analyticsService.GetTenantGrowthAsync();
        return Ok(growth);
    }

    [HttpGet("distribution")]
    public async Task<ActionResult> GetDistribution()
    {
        var distribution = await _analyticsService.GetSubscriptionDistributionAsync();
        return Ok(distribution);
    }
}
