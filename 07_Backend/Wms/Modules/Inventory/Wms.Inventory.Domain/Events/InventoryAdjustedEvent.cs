using Wms.Shared.Domain.Events;

namespace Wms.Inventory.Domain.Events;

/// <summary>
/// Inventory Adjusted Event (DE-006) — raised when inventory adjustment is executed.
/// Published by InventoryAdjustment aggregate.
/// Subscribed by Notification, CycleCount(v2.0).
/// </summary>
public class InventoryAdjustedEvent : EventDataBase
{
    public Guid AdjustmentId { get; set; }
    public string AdjustmentNo { get; set; } = string.Empty;
    public int AdjustmentTypeValue { get; set; }
    public Guid MaterialId { get; set; }
    public decimal AdjustmentQuantity { get; set; }
    public Guid WarehouseId { get; set; }
    public string SourceModule { get; set; } = "Inventory";
}
