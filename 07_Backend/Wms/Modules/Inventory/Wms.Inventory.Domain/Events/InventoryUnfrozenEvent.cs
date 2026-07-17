using Wms.Shared.Domain.Events;

namespace Wms.Inventory.Domain.Events;

/// <summary>
/// Inventory Unfrozen Event (DE-005) — raised when inventory is unfrozen/released.
/// Published by InventoryFreezeOrder aggregate.
/// Subscribed by Notification.
/// </summary>
public class InventoryUnfrozenEvent : EventDataBase
{
    public Guid FreezeOrderId { get; set; }
    public string FreezeOrderNo { get; set; } = string.Empty;
    public string ReleaseReason { get; set; } = string.Empty;
    public decimal UnfrozenQuantity { get; set; }
    public Guid WarehouseId { get; set; }
    public string SourceModule { get; set; } = "Inventory";
}
