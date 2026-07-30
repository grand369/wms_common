using Microsoft.AspNetCore.SignalR;

namespace Wms.Web.Host;

/// <summary>
/// WMS Task SignalR Hub — provides real-time task status updates,
/// assignment notifications, and monitoring data pushes.
/// </summary>
public class WmsTaskHub : Hub
{
    public async Task JoinWarehouseGroup(string warehouseCode)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"task-warehouse-{warehouseCode}");
    }

    public async Task LeaveWarehouseGroup(string warehouseCode)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"task-warehouse-{warehouseCode}");
    }

    public async Task JoinUserGroup(string userId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"task-user-{userId}");
    }

    public async Task LeaveUserGroup(string userId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"task-user-{userId}");
    }

    public override async Task OnConnectedAsync()
    {
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await base.OnDisconnectedAsync(exception);
    }
}
