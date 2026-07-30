using Microsoft.AspNetCore.SignalR;

namespace Wms.Web.Host;

/// <summary>
/// WMS Inventory SignalR Hub — provides real-time inventory change notifications.
/// Pushes stock updates, reservation changes, and freeze/unfreeze events.
/// </summary>
public class WmsInventoryHub : Hub
{
    public async Task JoinWarehouseGroup(string warehouseCode)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"inventory-warehouse-{warehouseCode}");
    }

    public async Task LeaveWarehouseGroup(string warehouseCode)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"inventory-warehouse-{warehouseCode}");
    }

    public async Task JoinMaterialGroup(string materialCode)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"inventory-material-{materialCode}");
    }

    public async Task LeaveMaterialGroup(string materialCode)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"inventory-material-{materialCode}");
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
