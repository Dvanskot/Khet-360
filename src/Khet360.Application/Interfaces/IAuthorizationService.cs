namespace Khet360.Application.Interfaces;

/// <summary>
/// Service for checking user permissions within the tenant.
/// </summary>
public interface IAuthorizationService
{
    /// <summary>
    /// Checks if the current user has the specified permission.
    /// </summary>
    bool HasPermission(string permissionCode);

    /// <summary>
    /// Checks if the current user has any of the specified permissions.
    /// </summary>
    bool HasAnyPermission(params string[] permissionCodes);

    /// <summary>
    /// Checks if the current user has all of the specified permissions.
    /// </summary>
    bool HasAllPermissions(params string[] permissionCodes);
}
