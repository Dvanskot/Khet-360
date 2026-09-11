using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Khet360.Application.Interfaces;

namespace Khet360.Api.Controllers.Platform;

[Authorize(AuthenticationSchemes = "PlatformJwt", Roles = "PlatformAdmin")]
[ApiController]
[Route("api/platform/features")]
public class PlatformFeatureController : ControllerBase
{
    private readonly IPlatformFeatureService _featureService;
    private readonly ILogger<PlatformFeatureController> _logger;

    public PlatformFeatureController(IPlatformFeatureService featureService, ILogger<PlatformFeatureController> logger)
    {
        _featureService = featureService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult> GetAllFeatures(Guid? tenantId = null)
    {
        var features = await _featureService.GetAllFeatureFlagsAsync(tenantId);
        return Ok(features);
    }

    [HttpPut("{featureId}/global")]
    public async Task<IActionResult> UpdateGlobalFeature(Guid featureId, [FromBody] bool isEnabled)
    {
        try
        {
            var username = User.Identity?.Name ?? "system";
            var success = await _featureService.UpdateGlobalFeatureAsync(featureId, isEnabled, username);
            if (!success) return NotFound(new { Message = "Feature not found." });
            return Ok(new { Message = "Feature updated successfully." });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { Message = ex.Message });
        }
    }

    [HttpPost("{tenantId}/override/{featureId}")]
    public async Task<IActionResult> SetTenantOverride(Guid tenantId, Guid featureId, [FromBody] bool isEnabled)
    {
        try
        {
            var username = User.Identity?.Name ?? "system";
            var success = await _featureService.SetTenantFeatureOverrideAsync(tenantId, featureId, isEnabled, username);
            if (!success) return NotFound(new { Message = "Feature or tenant not found." });
            return Ok(new { Message = "Tenant feature override set." });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { Message = ex.Message });
        }
    }

    [HttpDelete("{tenantId}/override/{featureId}")]
    public async Task<IActionResult> RemoveTenantOverride(Guid tenantId, Guid featureId)
    {
        try
        {
            var username = User.Identity?.Name ?? "system";
            var success = await _featureService.RemoveTenantFeatureOverrideAsync(tenantId, featureId, username);
            if (!success) return NotFound(new { Message = "Override not found." });
            return Ok(new { Message = "Tenant feature override removed." });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { Message = ex.Message });
        }
    }

    [HttpGet("tenant/{tenantId}")]
    public async Task<ActionResult> CheckTenantFeature(Guid tenantId, [FromQuery] string code)
    {
        var isEnabled = await _featureService.IsFeatureEnabledForTenantAsync(tenantId, code);
        return Ok(new { Code = code, IsEnabled = isEnabled });
    }
}
