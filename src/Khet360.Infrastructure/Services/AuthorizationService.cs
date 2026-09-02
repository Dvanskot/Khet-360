using Khet360.Application.Interfaces;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Khet360.Infrastructure.Persistence;
using System.Collections.Generic;

namespace Khet360.Infrastructure.Services;

public class AuthorizationService : IAuthorizationService
{
    private readonly ITenantUserContext _userContext;
    private readonly TenantDbContext _db;

    public AuthorizationService(ITenantUserContext userContext, TenantDbContext db)
    {
        _userContext = userContext;
        _db = db;
    }

    public async Task<bool> HasPermissionAsync(string permissionCode)
    {
        if (!_userContext.IsAuthenticated || _userContext.UserId == null) return false;

        var userId = _userContext.UserId.Value;

        // Check if user has a role that has the required permission
        var hasPermission = await _db.UserRoles
            .Where(ur => ur.UserId == userId)
            .Join(_db.RolePermissions,
                  ur => ur.RoleId,
                  rp => rp.RoleId,
                  (ur, rp) => rp.PermissionCode)
            .AnyAsync(code => code == permissionCode);

        return hasPermission;
    }

    public async Task<bool> HasPermissionInBranchAsync(string permissionCode, Guid branchId)
    {
        if (!_userContext.IsAuthenticated || _userContext.UserId == null) return false;

        var userId = _userContext.UserId.Value;

        // 1. Check if user has the permission via their roles
        var hasPermission = await _db.UserRoles
            .Where(ur => ur.UserId == userId)
            .Join(_db.RolePermissions,
                  ur => ur.RoleId,
                  rp => rp.RoleId,
                  (ur, rp) => rp.PermissionCode)
            .AnyAsync(code => code == permissionCode);

        if (!hasPermission) return false;

        // 2. Check if user is assigned to the specified branch
        var isAssignedToBranch = await _db.UserBranches
            .AnyAsync(ub => ub.UserId == userId && ub.BranchId == branchId);

        return isAssignedToBranch;
    }

    public bool HasPermission(string permissionCode)
    {
        // For synchronous checks, we can either use .Result (not recommended)
        // or assume the context already has the permissions.
        return _userContext.Permissions?.Contains(permissionCode) ?? false;
    }

    public bool HasAnyPermission(params string[] permissionCodes)
    {
        if (!_userContext.IsAuthenticated) return false;
        return permissionCodes.Any(code => _userContext.Permissions?.Contains(code) ?? false);
    }

    public bool HasAllPermissions(params string[] permissionCodes)
    {
        if (!_userContext.IsAuthenticated) return false;
        return permissionCodes.All(code => _userContext.Permissions?.Contains(code) ?? false);
    }
}
