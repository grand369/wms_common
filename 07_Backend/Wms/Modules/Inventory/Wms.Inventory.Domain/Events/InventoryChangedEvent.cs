using Wms.Shared.Domain.Events;
using Wms.Inventory.Domain.Enums;

namespace Wms.Inventory.Domain.Events;

/// <summary>
/// Inventory Changed Event (DE-001) — raised when inventory balance is modified.
/// Published by InventoryBalance.ApplyQuantityChange().
/// Subscribed by Notification, ERP(v2.0).
/// </summary>
public class InventoryChangedEvent : EventDataBase
{
    public Guid BalanceId { get; set; }
    public Guid MaterialId { get; set; }
    public Guid WarehouseId { get; set; }
    public decimal ChangeQuantity { get; set; }
    public decimal BeforeQuantity { get; set; }
    public decimal AfterQuantity { get; set; }
    public int OperationTypeValue { get; set; }
    public string SourceModule { get; set; } = "Inventory";
}
