using Microsoft.AspNetCore.Mvc;
using Khet360.Application.Interfaces;

namespace Khet360.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TenantController : ControllerBase
{
    private readonly ITenantService _tenantService;

    public TenantController(ITenantService tenantService)
    {
        _tenantService = tenantService;
    }

    [HttpGet("info")]
    public IActionResult GetTenantInfo()
    {
        var tenant = _tenantService.CurrentTenant;
        if (tenant == null)
        {
            return NotFound(new { Message = "No tenant resolved for the current request." });
        }

        return Ok(new
        {
            TenantId = tenant.Id,
            Name = tenant.Name,
            Slug = tenant.Slug,
            Plan = tenant.Plan
        });
    }
}
