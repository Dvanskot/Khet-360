using System;
using System.Threading.Tasks;
using Khet360.Application.Interfaces;

namespace Khet360.Infrastructure.Services;

public class StateSyncService : IStateSyncService
{
    private readonly IHubContextWrapper _hubWrapper;

    public StateSyncService(IHubContextWrapper hubWrapper)
    {
        _hubWrapper = hubWrapper;
    }

    public async Task NotifyStateChangedAsync(string entityType, Guid entityId, Guid branchId, string action)
    {
        await _hubWrapper.SendStateChangedAsync(branchId, entityType, entityId, action);
    }
}
