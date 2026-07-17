using Wms.Shared.Domain.Events;

namespace Wms.Inventory.Domain.Events;

/// <summary>
/// Safety Stock Alert Event (DE-002) — raised when available quantity falls below safety stock.
/// Published by InventoryDomainService.CheckSafetyStockAlert().
/// Subscribed by Notification.
/// </summary>
public class SafetyStockAlertEvent : EventDataBase
{
    public Guid MaterialId { get; set; }
    public string MaterialCode { get; set; } = string.Empty;
    public Guid WarehouseId { get; set; }
    public string WarehouseCode { get; set; } = string.Empty;
    public decimal CurrentAvailable { get; set; }
    public decimal SafetyStockQuantity { get; set; }
    public string SourceModule { get; set; } = "Inventory";
}
