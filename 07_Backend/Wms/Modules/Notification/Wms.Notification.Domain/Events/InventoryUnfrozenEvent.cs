using Wms.Shared.Domain.Events;

namespace Wms.Notification.Domain.Events;

/// <summary>
/// Inventory Unfrozen Event stub — matches Wms.Inventory.Domain.Events.InventoryUnfrozenEvent
/// </summary>
public class InventoryUnfrozenEvent : EventDataBase
{
    public Guid FreezeOrderId { get; set; }
    public string FreezeOrderNo { get; set; } = string.Empty;
    public string ReleaseReason { get; set; } = string.Empty;
    public decimal UnfrozenQuantity { get; set; }
    public Guid WarehouseId { get; set; }
}
