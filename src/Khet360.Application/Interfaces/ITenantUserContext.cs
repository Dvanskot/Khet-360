using Khet360.Domain.Entities;

namespace Khet360.Application.Interfaces;

public interface ITenantUserContext
{
    Guid? UserId { get; }
    IReadOnlyList<Guid> AssignedBranchIds { get; }
    IReadOnlyList<string> Roles { get; }
    IReadOnlyList<string> Permissions { get; }
    bool IsAuthenticated { get; }
}
