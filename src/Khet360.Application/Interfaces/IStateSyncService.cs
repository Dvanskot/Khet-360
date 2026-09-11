using System;
using Khet360.Domain.Entities.Common;
using Khet360.Domain.Entities.Platform;
using Khet360.Domain.Entities.Tenant;
using System.Threading.Tasks;

namespace Khet360.Application.Interfaces;

public interface IStateSyncService
{
    Task NotifyStateChangedAsync(string entityType, Guid entityId, Guid branchId, string action);
}
