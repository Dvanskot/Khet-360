using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Khet360.Application.Interfaces;
using Khet360.Application.Dtos;
using Khet360.Domain.Entities.Platform;

namespace Khet360.Api.Controllers.Platform;

[Authorize(AuthenticationSchemes = "PlatformJwt", Roles = "PlatformAdmin")]
[ApiController]
[Route("api/platform/tenants")]
public class PlatformTenantController : ControllerBase
{
    private readonly IPlatformTenantService _tenantService;
    private readonly ILogger<PlatformTenantController> _logger;

    public PlatformTenantController(IPlatformTenantService tenantService, ILogger<PlatformTenantController> logger)
    {
        _tenantService = tenantService;
        _logger = logger;
    }

    [HttpGet("{tenantId}")]
    public async Task<ActionResult<TenantDetailDto>> GetTenant(Guid tenantId)
    {
        try
        {
            var tenant = await _tenantService.GetTenantDetailsAsync(tenantId);
            return Ok(tenant);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { Message = ex.Message });
        }
    }

    [HttpGet("search")]
    public async Task<ActionResult<PagedResult<TenantListDto>>> SearchTenants(
        [FromQuery] string? searchTerm,
        [FromQuery] SubscriptionStatus? status,
        [FromQuery] IsolationTier? tier,
        [FromQuery] PlanCategory? planCategory,
        [FromQuery] bool? isActive,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var filter = new TenantSearchFilter(searchTerm, status, tier, planCategory, isActive, page, pageSize);
        var result = await _tenantService.SearchTenantsAsync(filter);
        return Ok(result);
    }

    [HttpPut("{tenantId}")]
    public async Task<IActionResult> UpdateTenant(Guid tenantId, [FromBody] UpdateTenantDto dto)
    {
        try
        {
            var username = User.Identity?.Name ?? "system";
            var success = await _tenantService.UpdateTenantAsync(tenantId, dto, username);
            if (!success) return NotFound(new { Message = "Tenant not found." });
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { Message = ex.Message });
        }
    }

    [HttpPost("{tenantId}/suspend")]
    public async Task<IActionResult> SuspendTenant(Guid tenantId, [FromBody] SuspendTenantDto dto)
    {
        try
        {
            var username = User.Identity?.Name ?? "system";
            var success = await _tenantService.SuspendTenantAsync(tenantId, dto.Reason, username);
            if (!success) return NotFound(new { Message = "Tenant not found." });
            return Ok(new { Message = "Tenant suspended successfully." });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { Message = ex.Message });
        }
    }

    [HttpPost("{tenantId}/cancel")]
    public async Task<IActionResult> CancelTenant(Guid tenantId, [FromBody] CancelTenantDto dto)
    {
        try
        {
            var username = User.Identity?.Name ?? "system";
            var success = await _tenantService.CancelTenantAsync(tenantId, dto.Reason, username);
            if (!success) return NotFound(new { Message = "Tenant not found." });
            return Ok(new { Message = "Tenant canceled successfully." });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { Message = ex.Message });
        }
    }

    [HttpPost("{tenantId}/reactivate")]
    public async Task<IActionResult> ReactivateTenant(Guid tenantId, [FromBody] ReactivateTenantDto dto)
    {
        try
        {
            var username = User.Identity?.Name ?? "system";
            var success = await _tenantService.ReactivateTenantAsync(tenantId, dto.Reason, username);
            if (!success) return NotFound(new { Message = "Tenant not found." });
            return Ok(new { Message = "Tenant reactivated successfully." });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { Message = ex.Message });
        }
    }
}
