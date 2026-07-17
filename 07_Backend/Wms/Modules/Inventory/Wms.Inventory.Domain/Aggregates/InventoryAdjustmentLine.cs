using Volo.Abp.Domain.Entities.Auditing;
using Wms.Shared.Domain.Enums;

namespace Wms.Inventory.Domain.Aggregates;

/// <summary>
/// Inventory Adjustment Line — child entity of InventoryAdjustment.
/// Represents a single line item in a stock adjustment.
/// </summary>
public class InventoryAdjustmentLine : FullAuditedEntity<Guid>
{
    /// <summary>Parent adjustment ID.</summary>
    public Guid AdjustmentId { get; private set; }

    /// <summary>Line number — sequential within the adjustment.</summary>
    public int LineNo { get; private set; }

    /// <summary>Material ID — reference to Material aggregate.</summary>
    public Guid MaterialId { get; private set; }

    /// <summary>Material code — redundant.</summary>
    public string MaterialCode { get; private set; }

    /// <summary>Material name — redundant.</summary>
    public string MaterialName { get; private set; }

    /// <summary>Adjustment quantity — positive for gain, negative for loss.</summary>
    public decimal AdjustmentQuantity { get; private set; }

    /// <summary>Location ID — where the adjustment applies.</summary>
    public Guid LocationId { get; private set; }

    /// <summary>Location code — redundant.</summary>
    public string LocationCode { get; private set; }

    /// <summary>Batch number — nullable.</summary>
    public string? BatchNumber { get; private set; }

    /// <summary>Inventory status before adjustment.</summary>
    public InventoryStatus InventoryStatusBefore { get; private set; }

    /// <summary>Inventory status after adjustment.</summary>
    public InventoryStatus InventoryStatusAfter { get; private set; }

    /// <summary>Line-level reason.</summary>
    public string? Reason { get; private set; }

    private InventoryAdjustmentLine() { }

    public InventoryAdjustmentLine(
        Guid id,
        Guid adjustmentId,
        int lineNo,
        Guid materialId,
        string materialCode,
        string materialName,
        decimal adjustmentQuantity,
        Guid locationId,
        string locationCode,
        string? batchNumber,
        InventoryStatus inventoryStatusBefore,
        InventoryStatus inventoryStatusAfter,
        string? reason = null)
        : base(id)
    {
        AdjustmentId = adjustmentId;
        LineNo = lineNo;
        MaterialId = materialId;
        MaterialCode = materialCode;
        MaterialName = materialName;
        AdjustmentQuantity = adjustmentQuantity;
        LocationId = locationId;
        LocationCode = locationCode;
        BatchNumber = batchNumber;
        InventoryStatusBefore = inventoryStatusBefore;
        InventoryStatusAfter = inventoryStatusAfter;
        Reason = reason;
    }
}
