using Volo.Abp.Domain.Entities.Auditing;
using Wms.Inventory.Domain.Enums;

namespace Wms.Inventory.Domain.Aggregates;

/// <summary>
/// Inventory Freeze Order Aggregate Root (AGG-09) — represents a freeze request
/// that locks inventory from being used. Can be released or cancelled after approval.
/// </summary>
public class InventoryFreezeOrder : FullAuditedAggregateRoot<Guid>
{
    /// <summary>Freeze order number — business natural key.</summary>
    public string FreezeOrderNo { get; private set; }

    /// <summary>Freeze scope — ByBatch/ByMaterial/ByLocation/ByWarehouse.</summary>
    public FreezeScope FreezeScope { get; private set; }

    /// <summary>Freeze reason — mandatory description.</summary>
    public string FreezeReason { get; private set; }

    /// <summary>Freeze status — Active/Released/Cancelled.</summary>
    public FreezeStatus FreezeStatus { get; private set; }

    /// <summary>Warehouse ID — where the freeze applies.</summary>
    public Guid WarehouseId { get; private set; }

    /// <summary>Warehouse code — redundant.</summary>
    public string WarehouseCode { get; private set; }

    /// <summary>Material ID — for single balance freeze.</summary>
    public Guid? MaterialId { get; private set; }

    /// <summary>Material code — redundant for display.</summary>
    public string? MaterialCode { get; private set; }

    /// <summary>Freeze quantity — for single balance freeze.</summary>
    public decimal FreezeQuantity { get; private set; }

    /// <summary>Whether the freeze order has been approved.</summary>
    public bool IsApproved { get; private set; }

    /// <summary>Freeze start time.</summary>
    public DateTime FreezeStartTime { get; private set; }

    /// <summary>Freeze end time — nullable (indefinite freeze).</summary>
    public DateTime? FreezeEndTime { get; private set; }

    /// <summary>Remark — optional note.</summary>
    public string? Remark { get; private set; }

    private InventoryFreezeOrder() { }

    public InventoryFreezeOrder(
        Guid id,
        string freezeOrderNo,
        FreezeScope freezeScope,
        string freezeReason,
        Guid warehouseId,
        string warehouseCode,
        DateTime freezeStartTime,
        DateTime? freezeEndTime = null,
        string? remark = null)
        : base(id)
    {
        FreezeOrderNo = freezeOrderNo;
        FreezeScope = freezeScope;
        FreezeReason = freezeReason;
        FreezeStatus = FreezeStatus.Active;
        WarehouseId = warehouseId;
        WarehouseCode = warehouseCode;
        MaterialId = null;
        MaterialCode = null;
        FreezeQuantity = 0m;
        IsApproved = false;
        FreezeStartTime = freezeStartTime;
        FreezeEndTime = freezeEndTime;
        Remark = remark;
    }

    /// <summary>
    /// Constructor for single balance freeze (manual freeze from balance list).
    /// </summary>
    public InventoryFreezeOrder(
        Guid id,
        string freezeOrderNo,
        FreezeScope freezeScope,
        string freezeReason,
        Guid warehouseId,
        string warehouseCode,
        Guid materialId,
        string materialCode,
        decimal freezeQuantity,
        DateTime freezeStartTime,
        DateTime? freezeEndTime = null,
        string? remark = null)
        : base(id)
    {
        FreezeOrderNo = freezeOrderNo;
        FreezeScope = freezeScope;
        FreezeReason = freezeReason;
        FreezeStatus = FreezeStatus.Active;
        WarehouseId = warehouseId;
        WarehouseCode = warehouseCode;
        MaterialId = materialId;
        MaterialCode = materialCode;
        FreezeQuantity = freezeQuantity;
        IsApproved = false;
        FreezeStartTime = freezeStartTime;
        FreezeEndTime = freezeEndTime;
        Remark = remark;
    }

    /// <summary>Approve the freeze order.</summary>
    public void Approve()
    {
        if (IsApproved)
        {
            throw new BusinessException("WMS:Inventory:FreezeAlreadyApproved",
                "Freeze order is already approved.");
        }
        if (FreezeStatus != FreezeStatus.Active)
        {
            throw new BusinessException("WMS:Inventory:FreezeNotActive",
                "Only active freeze orders can be approved.");
        }
        IsApproved = true;
    }

    /// <summary>Release the freeze — unfreeze the inventory.</summary>
    public void Release()
    {
        if (FreezeStatus != FreezeStatus.Active)
        {
            throw new BusinessException("WMS:Inventory:FreezeNotActive",
                "Only active freeze orders can be released.");
        }
        FreezeStatus = FreezeStatus.Released;
    }

    /// <summary>Cancel the freeze order.</summary>
    public void Cancel()
    {
        if (FreezeStatus == FreezeStatus.Released || FreezeStatus == FreezeStatus.Cancelled)
        {
            throw new BusinessException("WMS:Inventory:FreezeCannotCancel",
                "Cannot cancel a released or already cancelled freeze order.");
        }
        FreezeStatus = FreezeStatus.Cancelled;
    }
}
