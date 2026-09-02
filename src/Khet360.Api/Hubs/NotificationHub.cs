using Microsoft.AspNetCore.SignalR;
using System;
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
