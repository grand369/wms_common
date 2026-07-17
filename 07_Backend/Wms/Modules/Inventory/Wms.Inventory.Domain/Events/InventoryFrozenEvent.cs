using Wms.Shared.Domain.Events;

namespace Wms.Inventory.Domain.Events;

/// <summary>
/// Inventory Frozen Event (DE-004) — raised when inventory is frozen.
/// Published by InventoryFreezeOrder aggregate.
/// Subscribed by Notification.
/// </summary>
public class InventoryFrozenEvent : EventDataBase
{
    public Guid FreezeOrderId { get; set; }
    public string FreezeOrderNo { get; set; } = string.Empty;
    public int FreezeScopeValue { get; set; }
    public string FreezeReason { get; set; } = string.Empty;
    public decimal FrozenQuantity { get; set; }
    public Guid WarehouseId { get; set; }
    public string SourceModule { get; set; } = "Inventory";
}
