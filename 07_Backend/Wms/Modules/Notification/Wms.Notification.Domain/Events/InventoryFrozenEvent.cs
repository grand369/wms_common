using Wms.Shared.Domain.Events;

namespace Wms.Notification.Domain.Events;

/// <summary>
/// Inventory Frozen Event stub — matches Wms.Inventory.Domain.Events.InventoryFrozenEvent
/// </summary>
public class InventoryFrozenEvent : EventDataBase
{
    public Guid FreezeOrderId { get; set; }
    public string FreezeOrderNo { get; set; } = string.Empty;
    public int FreezeScopeValue { get; set; }
    public string FreezeReason { get; set; } = string.Empty;
    public decimal FrozenQuantity { get; set; }
    public Guid WarehouseId { get; set; }
}
