using Wms.Shared.Domain.Events;

namespace Wms.Inventory.Domain.Events;

/// <summary>
/// Zero Inventory Alert Event (DE-007) — raised when inventory reaches zero with pending demand.
/// Published by InventoryDomainService.
/// Subscribed by Notification.
/// </summary>
public class ZeroInventoryAlertEvent : EventDataBase
{
    public Guid MaterialId { get; set; }
    public string MaterialCode { get; set; } = string.Empty;
    public Guid WarehouseId { get; set; }
    public string WarehouseCode { get; set; } = string.Empty;
    public decimal PendingDemand { get; set; }
    public string SourceModule { get; set; } = "Inventory";
}
