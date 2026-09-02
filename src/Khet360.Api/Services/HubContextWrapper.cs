using System;
using System.Threading.Tasks;
using Khet360.Api.Hubs;
using Khet360.Application.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace Khet360.Api.Services;

public class HubContextWrapper : IHubContextWrapper
{
    private readonly IHubContext<NotificationHub> _hubContext;

    public HubContextWrapper(IHubContext<NotificationHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task SendStateChangedAsync(Guid branchId, string entityType, Guid entityId, string action)
    {
        await _hubContext.Clients.Group($"branch-{branchId}").SendAsync("StateChanged", new
        {
            EntityType = entityType,
            EntityId = entityId,
            Action = action,
            Timestamp = DateTime.UtcNow
        });
    }
}
