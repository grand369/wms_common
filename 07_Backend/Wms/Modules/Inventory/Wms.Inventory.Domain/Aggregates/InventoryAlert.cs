using Volo.Abp.Domain.Entities.Auditing;
using Wms.Inventory.Domain.Enums;

namespace Wms.Inventory.Domain.Aggregates;

/// <summary>
/// Inventory Alert Aggregate Root — represents a generated alert for inventory anomalies
/// (safety stock breach, near expiry, zero inventory, overstock, age threshold).
/// Can be resolved once the condition is addressed.
/// </summary>
public class InventoryAlert : FullAuditedAggregateRoot<Guid>
{
    /// <summary>Alert type — SafetyStock/Expiry/ZeroInventory/Overstock/Age.</summary>
    public AlertType AlertType { get; private set; }

    /// <summary>Material ID — the material that triggered the alert.</summary>
    public Guid MaterialId { get; private set; }

    /// <summary>Material code — redundant.</summary>
    public string MaterialCode { get; private set; }

    /// <summary>Warehouse ID — where the alert condition exists.</summary>
    public Guid WarehouseId { get; private set; }

    /// <summary>Warehouse code — redundant.</summary>
    public string WarehouseCode { get; private set; }

    /// <summary>Current quantity at the time of alert.</summary>
    public decimal CurrentQuantity { get; private set; }

    /// <summary>Threshold quantity — the configured threshold that was breached.</summary>
    public decimal ThresholdQuantity { get; private set; }

    /// <summary>Whether the alert has been resolved.</summary>
    public bool IsResolved { get; private set; }

    /// <summary>Alert timestamp.</summary>
    public DateTime AlertTime { get; private set; }

    /// <summary>Resolution timestamp.</summary>
    public DateTime? ResolveTime { get; private set; }

    private InventoryAlert() { }

    public InventoryAlert(
        Guid id,
        AlertType alertType,
        Guid materialId,
        string materialCode,
        Guid warehouseId,
        string warehouseCode,
        decimal currentQuantity,
        decimal thresholdQuantity)
        : base(id)
    {
        AlertType = alertType;
        MaterialId = materialId;
        MaterialCode = materialCode;
        WarehouseId = warehouseId;
        WarehouseCode = warehouseCode;
        CurrentQuantity = currentQuantity;
        ThresholdQuantity = thresholdQuantity;
        IsResolved = false;
        AlertTime = DateTime.UtcNow;
        ResolveTime = null;
    }

    /// <summary>Resolve the alert — marks it as addressed.</summary>
    public void Resolve()
    {
        if (IsResolved)
        {
            throw new BusinessException("WMS:Inventory:AlertAlreadyResolved",
                "Alert is already resolved.");
        }
        IsResolved = true;
        ResolveTime = DateTime.UtcNow;
    }
}
