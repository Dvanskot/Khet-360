using Khet360.Application.Interfaces;
using Khet360.Domain.Entities.Tenant;
using Khet360.Domain.Entities.Platform;
using Khet360.Domain.Entities.Common;
using Khet360.Infrastructure.Persistence;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Khet360.AuthApi.Middleware;

public class TenantResolverMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<TenantResolverMiddleware> _logger;

    public TenantResolverMiddleware(RequestDelegate next, ILogger<TenantResolverMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, PlatformDbContext platformDb, ITenantService tenantService, IPlatformCacheService cache)
    {
        var host = context.Request.Host.Host;
        if (string.IsNullOrEmpty(host))
        {
            await _next(context);
            return;
        }

        // Extract subdomain: tenanta.khet360.co.za -> tenanta
        // Support localhost: tenanta.localhost -> tenanta
        var slug = ExtractSlug(host);

        if (!string.IsNullOrEmpty(slug))
        {
            var tenants = await cache.GetTenantsAsync();
            var tenant = tenants.FirstOrDefault(t => t.Slug == slug);

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

    private string? ExtractSlug(string host)
    {
        // Handle localhost explicitly
        if (host.EndsWith(".localhost"))
        {
            return host.Replace(".localhost", "");
        }

        var parts = host.Split('.');
        if (parts.Length >= 3)
        {
            return parts[0];
        }

        return null;
    }
}