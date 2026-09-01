using Microsoft.AspNetCore.Mvc;
using Khet360.Application.Interfaces;

namespace Khet360.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly ITenantAuthService _authService;
    private readonly ITenantService _tenantService;

    public AuthController(ITenantAuthService authService, ITenantService tenantService)
    {
        _authService = authService;
        _tenantService = tenantService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var tenant = _tenantService.CurrentTenant;
        if (tenant == null)
        {
            return NotFound(new { Message = "No tenant resolved for the current request." });
        }

        var response = await _authService.LoginAsync(request.Username, request.Password, tenant.Id);
        if (response == null)
        {
            return Unauthorized(new { Message = "Invalid credentials." });
        }

        return Ok(response);
    }
}

public record LoginRequest(string Username, string Password);
