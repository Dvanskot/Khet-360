using Khet360.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Khet360.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class TenantAnalyticsController : ControllerBase
{
    private readonly ITenantAnalyticsService _analyticsService;

    public TenantAnalyticsController(ITenantAnalyticsService analyticsService)
    {
        _analyticsService = analyticsService;
    }

    [HttpGet("operational-efficiency")]
    public async Task<IActionResult> GetOperationalEfficiency()
    {
        var result = await _analyticsService.GetOperationalEfficiencyAsync();
        return Ok(result);
    }

    [HttpGet("branch-performance")]
    public async Task<IActionResult> GetBranchPerformance()
    {
        var result = await _analyticsService.GetBranchPerformanceAsync();
        return Ok(result);
    }

    [HttpGet("sla-compliance")]
    public async Task<IActionResult> GetSlaCompliance()
    {
        var result = await _analyticsService.GetSlaComplianceAsync();
        return Ok(result);
    }

    [HttpGet("workload-distribution")]
    public async Task<IActionResult> GetWorkloadDistribution()
    {
        var result = await _analyticsService.GetWorkloadDistributionAsync();
        return Ok(result);
    }
}
