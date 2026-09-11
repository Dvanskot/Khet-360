using Khet360.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Khet360.AuthApi.Middleware;

public class TenantBindingMiddleware
{
    private readonly RequestDelegate _next;

    public TenantBindingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, ITenantService tenantService)
    {
        var tenant = tenantService.CurrentTenant;
        if (tenant != null
            && context.User.Identity?.IsAuthenticated == true
            && IsTenantPlane(context.Request.Path)
            && !IsBoundToCurrentTenant(context.User, tenant.Id))
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            await context.Response.WriteAsync("Token is not bound to the resolved tenant.");
            return;
        }

        await _next(context);
    }

    private static bool IsTenantPlane(PathString path)
    {
        return path.StartsWithSegments("/api")
            && !path.StartsWithSegments("/api/platform")
            && !path.StartsWithSegments("/api/auth/platform");
    }

    private static bool IsBoundToCurrentTenant(ClaimsPrincipal user, Guid tenantId)
    {
        var claim = user.FindFirstValue("tenant_id");
        return Guid.TryParse(claim, out var tokenTenantId) && tokenTenantId == tenantId;
    }
}