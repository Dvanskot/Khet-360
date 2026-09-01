using System.ComponentModel.DataAnnotations;

namespace Khet360.Domain.Entities;

/// <summary>
/// Join entity mapping Users to the Branches they are assigned to.
/// </summary>
public class UserBranch
{
    [Key]
    public Guid UserId { get; set; }

    [Key]
    public Guid BranchId { get; set; }

    public virtual User User { get; set; } = null!;
    public virtual Branch Branch { get; set; } = null!;
}
