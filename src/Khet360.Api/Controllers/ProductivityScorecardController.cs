using Khet360.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Khet360.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductivityScorecardController : ControllerBase
{
    private readonly IProductivityScorecardService _scorecardService;

    public ProductivityScorecardController(IProductivityScorecardService scorecardService)
    {
        _scorecardService = scorecardService;
    }

    [HttpGet]
    public async Task<IActionResult> GetScorecard([FromQuery] Guid branchId)
    {
        var scorecard = await _scorecardService.GetScorecardAsync(branchId);
        return Ok(scorecard);
    }
}
