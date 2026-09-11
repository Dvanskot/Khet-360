using Khet360.Application.Dtos;
using Khet360.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Khet360.AuthApi.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly ITenantAuthService _authService;
    private readonly IPlatformAuthService _platformAuthService;
    private readonly ITenantService _tenantService;

    public AuthController(
        ITenantAuthService authService,
        IPlatformAuthService platformAuthService,
        ITenantService tenantService)
    {
        _authService = authService;
        _platformAuthService = platformAuthService;
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

    [HttpPost("platform/login")]
    public async Task<IActionResult> PlatformLogin([FromBody] LoginRequest request)
    {
        var response = await _platformAuthService.LoginAsync(request.Username, request.Password);
        if (response == null)
        {
            return Unauthorized(new { Message = "Invalid credentials." });
        }

        return Ok(response);
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest request)
    {
        try
        {
            var token = await _authService.RefreshTokenAsync(request.Token, request.RefreshToken);
            return Ok(new { Token = token });
        }
        catch
        {
            return Unauthorized(new { Message = "Invalid refresh token." });
        }
    }

    [HttpPost("platform/refresh")]
    public async Task<IActionResult> PlatformRefresh([FromBody] RefreshTokenRequest request)
    {
        var response = await _platformAuthService.RefreshTokenAsync(request.Token, request.RefreshToken);
        if (response == null)
        {
            return Unauthorized(new { Message = "Invalid refresh token." });
        }

        return Ok(response);
    }

    [Authorize(AuthenticationSchemes = "TenantJwt")]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout([FromBody] LogoutRequest request)
    {
        var accessToken = Request.Headers.Authorization.ToString();
        if (accessToken.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
        {
            accessToken = accessToken["Bearer ".Length..].Trim();
        }

        var success = await _authService.LogoutAsync(accessToken, request.RefreshToken);
        return success ? Ok(new { Message = "Signed out." }) : Unauthorized(new { Message = "Invalid refresh token." });
    }

    [Authorize(AuthenticationSchemes = "PlatformJwt")]
    [HttpPost("platform/logout")]
    public async Task<IActionResult> PlatformLogout([FromBody] LogoutRequest request)
    {
        var accessToken = Request.Headers.Authorization.ToString();
        if (accessToken.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
        {
            accessToken = accessToken["Bearer ".Length..].Trim();
        }

        var success = await _platformAuthService.LogoutAsync(accessToken, request.RefreshToken);
        return success ? Ok(new { Message = "Signed out." }) : Unauthorized(new { Message = "Invalid refresh token." });
    }
}
