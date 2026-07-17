using Wms.Shared.Domain.Events;

namespace Wms.Inventory.Domain.Events;

/// <summary>
/// Expiry Alert Event (DE-003) — raised when inventory is near expiry date.
/// Published by InventoryDomainService.CheckExpiryAlert().
/// Subscribed by Notification.
/// </summary>
public class ExpiryAlertEvent : EventDataBase
{
    public Guid MaterialId { get; set; }
    public string MaterialCode { get; set; } = string.Empty;
    public Guid WarehouseId { get; set; }
    public string WarehouseCode { get; set; } = string.Empty;
    public string BatchNumber { get; set; } = string.Empty;
    public DateTime ExpiryDate { get; set; }
    public int DaysLeft { get; set; }
    public string SourceModule { get; set; } = "Inventory";
}
