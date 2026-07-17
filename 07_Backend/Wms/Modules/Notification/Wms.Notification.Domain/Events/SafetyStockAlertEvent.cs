using Wms.Shared.Domain.Events;

namespace Wms.Notification.Domain.Events;

/// <summary>
/// Safety Stock Alert Event stub — matches Wms.Inventory.Domain.Events.SafetyStockAlertEvent
/// </summary>
public class SafetyStockAlertEvent : EventDataBase
{
    public Guid MaterialId { get; set; }
    public string MaterialCode { get; set; } = string.Empty;
    public Guid WarehouseId { get; set; }
    public string WarehouseCode { get; set; } = string.Empty;
    public decimal CurrentAvailable { get; set; }
    public decimal SafetyStockQuantity { get; set; }
}
