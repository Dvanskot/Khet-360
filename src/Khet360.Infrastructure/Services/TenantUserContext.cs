using Khet360.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Khet360.Infrastructure.Services;

public class TenantUserContext : ITenantUserContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public TenantUserContext(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid? UserId => GetClaimValue<Guid>("sub");
    public IReadOnlyList<Guid> AssignedBranchIds => GetGuidClaims("branch_id");
    public IReadOnlyList<string> Roles => _httpContextAccessor.HttpContext?.User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList() ?? new List<string>();
    public IReadOnlyList<string> Permissions => _httpContextAccessor.HttpContext?.User.FindAll("permission").Select(c => c.Value).ToList() ?? new List<string>();
    public bool IsAuthenticated => _httpContextAccessor.HttpContext?.User.Identity?.IsAuthenticated ?? false;

    private IReadOnlyList<Guid> GetGuidClaims(string claimType)
    {
        var context = _httpContextAccessor.HttpContext;
        if (context?.User == null)
        {
            return Array.Empty<Guid>();
        }

        return context.User.FindAll(claimType)
            .Select(claim => claim.Value)
            .Select(value => Guid.TryParse(value, out var id) ? id : (Guid?)null)
            .Where(value => value.HasValue)
            .Select(value => value!.Value)
            .Distinct()
            .ToList();
    }

    private Guid? GetClaimValue<T>(string claimType) where T : struct
    {
        var value = _httpContextAccessor.HttpContext?.User.FindFirstValue(claimType);
        if (string.IsNullOrEmpty(value))
        {
            return null;
        }

        if (typeof(T) == typeof(Guid) && Guid.TryParse(value, out var guid))
        {
            return (Guid)(object)guid;
        }

        return null;
    }
}
