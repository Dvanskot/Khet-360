using System.ComponentModel.DataAnnotations;

namespace Khet360.Domain.Entities;

/// <summary>
/// Join entity mapping Roles to their associated Permissions.
/// </summary>
public class RolePermission
{
    [Key]
    public Guid RoleId { get; set; }

    [Key]
    public string PermissionCode { get; set; } = null!;

    public virtual Role Role { get; set; } = null!;
    public virtual Permission Permission { get; set; } = null!;
}
