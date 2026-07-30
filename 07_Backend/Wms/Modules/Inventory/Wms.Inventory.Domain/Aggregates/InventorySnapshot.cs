using Volo.Abp.Domain.Entities.Auditing;

namespace Wms.Inventory.Domain.Aggregates;

/// <summary>
/// Inventory Snapshot (AGG-10) — a point-in-time snapshot of inventory balances.
/// Supports generating snapshots by warehouse for inventory verification purposes.
/// </summary>
public class InventorySnapshot : FullAuditedAggregateRoot<Guid>
{
    /// <summary>Snapshot number — business natural key.</summary>
    public string SnapshotNo { get; private set; }

    /// <summary>Warehouse ID — the warehouse scope of this snapshot.</summary>
    public Guid WarehouseId { get; private set; }

    /// <summary>Warehouse code — redundant for display.</summary>
    public string WarehouseCode { get; private set; }

    /// <summary>Snapshot time — when the snapshot was taken.</summary>
    public DateTime SnapshotTime { get; private set; }

    /// <summary>Total quantity — sum of all balance quantities in the snapshot.</summary>
    public decimal TotalQty { get; private set; }

    /// <summary>Total frozen quantity — sum of all frozen quantities.</summary>
    public decimal TotalFrozenQty { get; private set; }

    /// <summary>Total available quantity — sum of all available quantities.</summary>
    public decimal TotalAvailableQty { get; private set; }

    /// <summary>Status — 0=Pending, 1=Completed.</summary>
    public int Status { get; private set; }

    /// <summary>Remark — optional note.</summary>
    public string? Remark { get; private set; }

    private InventorySnapshot() { }

    public InventorySnapshot(
        Guid id,
        string snapshotNo,
        Guid warehouseId,
        string warehouseCode,
        DateTime snapshotTime,
        decimal totalQty,
        decimal totalFrozenQty,
        decimal totalAvailableQty,
        string? remark = null)
        : base(id)
    {
        SnapshotNo = snapshotNo;
        WarehouseId = warehouseId;
        WarehouseCode = warehouseCode;
        SnapshotTime = snapshotTime;
        TotalQty = totalQty;
        TotalFrozenQty = totalFrozenQty;
        TotalAvailableQty = totalAvailableQty;
        Status = 1; // Completed
        Remark = remark;
    }
}
