using Microsoft.AspNetCore.SignalR;

namespace Wms.Web.Host;

/// <summary>
/// WMS Notification SignalR Hub — provides real-time push notifications
/// for task assignments, inventory alerts, and workflow status changes.
/// </summary>
public class WmsNotificationHub : Hub
{
    /// <summary>
    /// Client joins a warehouse-specific notification group.
    /// </summary>
    public async Task JoinWarehouseGroup(string warehouseCode)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"warehouse-{warehouseCode}");
    }

    /// <summary>
    /// Client leaves a warehouse-specific notification group.
    /// </summary>
    public async Task LeaveWarehouseGroup(string warehouseCode)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"warehouse-{warehouseCode}");
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
