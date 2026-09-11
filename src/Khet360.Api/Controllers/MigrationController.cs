using Khet360.Application.Dtos;
using Khet360.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using Khet360.Domain.Entities.Common;
using Khet360.Domain.Entities.Platform;
using Khet360.Domain.Entities.Tenant;
using System.Threading.Tasks;

namespace Khet360.Api.Controllers;

[ApiController]
[Authorize(AuthenticationSchemes = "PlatformJwt", Roles = "PlatformAdmin")]
[Route("api/[controller]")]
public class MigrationController : ControllerBase
{
    private readonly IMigrationService _migrationService;

    public MigrationController(IMigrationService migrationService)
    {
        _migrationService = migrationService;
    }

    [HttpPost("migrate")]
    public async Task<IActionResult> MigrateTenant([FromBody] MigrationRequest request)
    {
        var success = await _migrationService.MigrateTenantAsync(request.TenantId, request.TargetEnvironment);
        if (!success) return BadRequest("Migration failed. Check logs for details.");
        return Ok(new { Message = "Migration completed successfully." });
    }
}
