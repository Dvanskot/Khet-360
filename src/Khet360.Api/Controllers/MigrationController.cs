using Khet360.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Khet360.Api.Controllers;

[ApiController]
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

public record MigrationRequest(Guid TenantId, string TargetEnvironment);
