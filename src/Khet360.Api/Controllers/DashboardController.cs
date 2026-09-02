using Microsoft.AspNetCore.Mvc;
using Khet360.Application.Interfaces;
using Khet360.Application.Dtos;
using System.Threading.Tasks;

namespace Khet360.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DashboardController : ControllerBase
{
    private readonly IDashboardService _dashboardService;

    public DashboardController(IDashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }

    [HttpGet("data")]
    public async Task<IActionResult> GetDashboardData([FromQuery] Guid userId)
    {
        var data = await _dashboardService.GetDashboardDataAsync(userId);
        return Ok(data);
    }

    [HttpGet("layout")]
    public async Task<ActionResult<UserDashboardLayoutDto>> GetLayout([FromQuery] Guid userId)
    {
        var layout = await _dashboardService.GetUserLayoutAsync(userId);
        return Ok(layout);
    }

    [HttpPost("layout")]
    public async Task<IActionResult> SaveLayout([FromBody] UserDashboardLayoutDto layout)
    {
        await _dashboardService.SaveUserLayoutAsync(layout);
        return Ok();
    }
}
