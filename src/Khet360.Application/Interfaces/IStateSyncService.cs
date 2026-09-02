using System;
using System.Threading.Tasks;

namespace Khet360.Application.Interfaces;

public interface IStateSyncService
{
    Task NotifyStateChangedAsync(string entityType, Guid entityId, Guid branchId, string action);
}
