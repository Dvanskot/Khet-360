
using Khet360.Domain.Entities.Tenant;
using Khet360.Domain.Entities.Platform;
using Khet360.Domain.Entities.Common;
namespace Khet360.Application.Interfaces;

public interface ITenantUserContext
{
    Guid? UserId { get; }
    IReadOnlyList<Guid> AssignedBranchIds { get; }
    IReadOnlyList<string> Roles { get; }
    IReadOnlyList<string> Permissions { get; }
    bool IsAuthenticated { get; }
}
