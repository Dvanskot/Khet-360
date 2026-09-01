using Khet360.Application.Interfaces;
using System.Linq;

namespace Khet360.Infrastructure.Services;

public class AuthorizationService : IAuthorizationService
{
    private readonly ITenantUserContext _userContext;

    public AuthorizationService(ITenantUserContext userContext)
    {
        _userContext = userContext;
    }

    public bool HasPermission(string permissionCode)
    {
        if (!_userContext.IsAuthenticated) return false;
        return _userContext.Permissions.Contains(permissionCode);
    }

    public bool HasAnyPermission(params string[] permissionCodes)
    {
        if (!_userContext.IsAuthenticated) return false;
        return permissionCodes.Any(code => _userContext.Permissions.Contains(code));
    }

    public bool HasAllPermissions(params string[] permissionCodes)
    {
        if (!_userContext.IsAuthenticated) return false;
        return permissionCodes.All(code => _userContext.Permissions.Contains(code));
    }
}
