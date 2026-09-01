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

    [HttpGet("operational-overview")]
    public async Task<ActionResult<OperationalDashboardDto>> GetOperationalOverview()
    {
        var overview = await _dashboardService.GetOperationalOverviewAsync();
        return Ok(overview);
    }
}
