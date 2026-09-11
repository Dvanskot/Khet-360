using Microsoft.AspNetCore.SignalR;
using System;
using Khet360.Domain.Entities.Common;
using Khet360.Domain.Entities.Platform;
using Khet360.Domain.Entities.Tenant;
using System.Threading.Tasks;

namespace Khet360.Api.Hubs;

public class NotificationHub : Hub
{
    public async Task JoinBranchGroup(Guid branchId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"branch-{branchId}");
    }

    public async Task LeaveBranchGroup(Guid branchId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"branch-{branchId}");
    }
}
