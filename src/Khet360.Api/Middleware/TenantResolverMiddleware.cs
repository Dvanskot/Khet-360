using Khet360.Application.Interfaces;
using Khet360.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Khet360.Api.Middleware;

public class TenantResolverMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<TenantResolverMiddleware> _logger;

    public TenantResolverMiddleware(RequestDelegate next, ILogger<TenantResolverMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, PlatformDbContext platformDb, ITenantService tenantService)
    {
        var host = context.Request.Host.Host;
        if (string.IsNullOrEmpty(host))
        {
            await _next(context);
            return;
        }

        // Logic to extract subdomain (e.g., tenanta.khet360.co.za -> tenanta)
        // This is a simplified version for development.
        var parts = host.Split('.');
        if (parts.Length >= 3)
        {
            var slug = parts[0];

            var tenant = await platformDb.Tenants
                .FirstOrDefaultAsync(t => t.Slug == slug && t.IsActive);

            if (tenant == null)
            {
                _logger.LogWarning("Tenant with slug {Slug} not found or inactive.", slug);
                context.Response.StatusCode = StatusCodes.Status404NotFound;
                await context.Response.WriteAsync("Tenant not found or inactive.");
                return;
            }

            tenantService.SetTenant(tenant);
            _logger.LogInformation("Resolved tenant: {TenantName} ({Slug})", tenant.Name, tenant.Slug);
        }
        else
        {
            _logger.LogDebug("No tenant subdomain detected in host {Host}", host);
        }

        await _next(context);
    }
}
