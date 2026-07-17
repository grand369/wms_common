using Volo.Abp.Domain.Entities;
using Wms.Inventory.Domain.Enums;

namespace Wms.Inventory.Domain.Aggregates;

/// <summary>
/// Inventory Ledger Entry (AGG-07) — immutable record of every inventory change.
/// ⚠️ CRITICAL: This entity is NOT modifiable or deletable (BR-010).
/// Inherits Entity<Guid> (NOT FullAuditedAggregateRoot) — only has CreationTime/CreatorId.
/// Repository Update/Delete methods are overridden to throw NotSupportedException.
/// </summary>
public class InventoryLedgerEntry : Entity<Guid>, IHasCreationTime
{
    /// <summary>Associated inventory balance ID.</summary>
    public Guid InventoryBalanceId { get; private set; }

    /// <summary>Operation type — 10 types of inventory operations.</summary>
    public InventoryOperationType OperationType { get; private set; }

    /// <summary>Operation quantity — positive for increases, negative for decreases (or always positive with type indicating direction).</summary>
    public decimal OperationQuantity { get; private set; }

    /// <summary>Quantity before the operation.</summary>
    public decimal BeforeQuantity { get; private set; }

    /// <summary>Quantity after the operation.</summary>
    public decimal AfterQuantity { get; private set; }

    /// <summary>Available quantity before the operation.</summary>
    public decimal BeforeAvailable { get; private set; }

    /// <summary>Available quantity after the operation.</summary>
    public decimal AfterAvailable { get; private set; }

    /// <summary>Operation timestamp.</summary>
    public DateTime OperationTime { get; private set; }

    /// <summary>Operator ID — who performed the operation.</summary>
    public Guid OperatorId { get; private set; }

    /// <summary>Operator name — redundant for query optimization.</summary>
    public string OperatorName { get; private set; }

    /// <summary>Source order type — InboundOrder/OutboundOrder/TransferOrder/CycleCount/InventoryAdjustment/InventoryFreeze.</summary>
    public string SourceOrderType { get; private set; }

    /// <summary>Source order ID — reference to the business document.</summary>
    public Guid SourceOrderId { get; private set; }

    /// <summary>Source order number — redundant for query optimization.</summary>
    public string SourceOrderNo { get; private set; }

    /// <summary>Remark — optional note.</summary>
    public string? Remark { get; private set; }

    /// <summary>Creation time — IHasCreationTime for ABP audit.</summary>
    public DateTime CreationTime { get; set; }

    /// <summary>Creator ID — for ABP audit.</summary>
    public Guid? CreatorId { get; set; }

    // ⚠️ No LastModificationTime, IsDeleted, etc. — this entity is immutable.

    private InventoryLedgerEntry() { }

    public InventoryLedgerEntry(
        Guid id,
        Guid inventoryBalanceId,
        InventoryOperationType operationType,
        decimal operationQuantity,
        decimal beforeQuantity,
        decimal afterQuantity,
        decimal beforeAvailable,
        decimal afterAvailable,
        DateTime operationTime,
        Guid operatorId,
        string operatorName,
        string sourceOrderType,
        Guid sourceOrderId,
        string sourceOrderNo,
        string? remark)
        : base(id)
    {
        InventoryBalanceId = inventoryBalanceId;
        OperationType = operationType;
        OperationQuantity = operationQuantity;
        BeforeQuantity = beforeQuantity;
        AfterQuantity = afterQuantity;
        BeforeAvailable = beforeAvailable;
        AfterAvailable = afterAvailable;
        OperationTime = operationTime;
        OperatorId = operatorId;
        OperatorName = operatorName ?? string.Empty;
        SourceOrderType = sourceOrderType;
        SourceOrderId = sourceOrderId;
        SourceOrderNo = sourceOrderNo ?? string.Empty;
        Remark = remark;
    }
}
