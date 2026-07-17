using Wms.Shared.Domain.Events;

namespace Wms.Notification.Domain.Events;

/// <summary>
/// Zero Inventory Alert Event stub — matches Wms.Inventory.Domain.Events.ZeroInventoryAlertEvent
/// </summary>
public class ZeroInventoryAlertEvent : EventDataBase
{
    public Guid MaterialId { get; set; }
    public string MaterialCode { get; set; } = string.Empty;
    public Guid WarehouseId { get; set; }
    public string WarehouseCode { get; set; } = string.Empty;
    public decimal PendingDemand { get; set; }
}
