using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.EventBus.Local;
using Wms.Shared.Domain.Enums;
using Wms.Inventory.Domain.Enums;
using Wms.Inventory.Domain.Events;
using Wms.Inventory.Domain.StateMachines;
using Wms.Inventory.Domain.ValueObjects;

namespace Wms.Inventory.Domain.Aggregates;

/// <summary>
/// Inventory Balance Aggregate Root (AGG-06) — the core aggregate of the entire platform.
/// Represents the current stock position for a specific material+warehouse+location+batch+status combination.
/// Unique key: (MaterialId, WarehouseId, LocationId, BatchNumber, InventoryStatus).
/// All inventory quantity changes go through ApplyQuantityChange() method (BR-001).
/// Uses optimistic locking via ConcurrencyVersion (IsConcurrencyToken).
/// </summary>
public class InventoryBalance : FullAuditedAggregateRoot<Guid>
{
    /// <summary>Material ID — reference to Material aggregate.</summary>
    public Guid MaterialId { get; private set; }

    /// <summary>Material code — redundant field for query optimization.</summary>
    public string MaterialCode { get; private set; }

    /// <summary>Warehouse ID — reference to Warehouse aggregate.</summary>
    public Guid WarehouseId { get; private set; }

    /// <summary>Warehouse code — redundant field for query optimization.</summary>
    public string WarehouseCode { get; private set; }

    /// <summary>Location ID — reference to Location aggregate.</summary>
    public Guid LocationId { get; private set; }

    /// <summary>Location code — redundant field for query optimization.</summary>
    public string LocationCode { get; private set; }

    /// <summary>Batch number — nullable, required for batch-managed materials.</summary>
    public string? BatchNumber { get; private set; }

    /// <summary>Inventory status — uses shared kernel InventoryStatus SmartEnum.</summary>
    public InventoryStatus InventoryStatus { get; private set; }

    /// <summary>Total quantity in this balance.</summary>
    public decimal Quantity { get; private set; }

    /// <summary>Reserved quantity — allocated but not yet shipped.</summary>
    public decimal ReservedQuantity { get; private set; }

    /// <summary>Frozen quantity — frozen and not available for operations.</summary>
    public decimal FrozenQuantity { get; private set; }

    /// <summary>In-transit quantity — being transferred between warehouses.</summary>
    public decimal InTransitQuantity { get; private set; }

    /// <summary>Available quantity = Quantity - ReservedQuantity - FrozenQuantity (computed by code, NOT DB).</summary>
    public decimal AvailableQuantity { get; private set; }

    /// <summary>Expiry date — nullable, required for expiry-managed materials.</summary>
    public DateTime? ExpiryDate { get; private set; }

    /// <summary>Production date.</summary>
    public DateTime? ProductionDate { get; private set; }

    /// <summary>Supplier ID — nullable reference.</summary>
    public Guid? SupplierId { get; private set; }

    /// <summary>Supplier name — redundant field.</summary>
    public string? SupplierName { get; private set; }

    /// <summary>Unit cost — for inventory value calculation.</summary>
    public decimal? UnitCost { get; private set; }

    /// <summary>Last operation time — tracks when the balance was last modified.</summary>
    public DateTime LastOperationTime { get; private set; }

    /// <summary>Concurrency version — optimistic lock token for EF Core.</summary>
    public int ConcurrencyVersion { get; set; }

    private InventoryBalance() { }

    public InventoryBalance(
        Guid id,
        Guid materialId,
        string materialCode,
        Guid warehouseId,
        string warehouseCode,
        Guid locationId,
        string locationCode,
        string? batchNumber,
        InventoryStatus inventoryStatus)
        : base(id)
    {
        MaterialId = materialId;
        MaterialCode = materialCode;
        WarehouseId = warehouseId;
        WarehouseCode = warehouseCode;
        LocationId = locationId;
        LocationCode = locationCode;
        BatchNumber = batchNumber;
        InventoryStatus = inventoryStatus;
        Quantity = 0m;
        ReservedQuantity = 0m;
        FrozenQuantity = 0m;
        InTransitQuantity = 0m;
        AvailableQuantity = 0m;
        LastOperationTime = DateTime.UtcNow;
        ConcurrencyVersion = 0;
    }

    /// <summary>
    /// ⚠️ Core method — ApplyQuantityChange. This is the ONLY method that modifies quantity fields.
    /// Based on operation type, updates the appropriate quantity field and recalculates AvailableQuantity.
    /// Publishes InventoryChangedEvent (DE-001).
    /// Returns InventoryChangeResult containing the generated ledger entry.
    /// </summary>
    public InventoryChangeResult ApplyQuantityChange(
        InventoryOperationType operationType,
        decimal changeQuantity,
        string sourceOrderType,
        Guid sourceOrderId,
        bool allowNegativeInventory = false,
        string? sourceOrderNo = null)
    {
        var beforeQuantity = Quantity;
        var beforeAvailable = AvailableQuantity;

        if (operationType == InventoryOperationType.InboundIncrease
            || operationType == InventoryOperationType.AdjustIncrease
            || operationType == InventoryOperationType.TransferIn
            || operationType == InventoryOperationType.ReplenishmentIncrease)
        {
            Quantity += changeQuantity;
        }
        else if (operationType == InventoryOperationType.OutboundDecrease
            || operationType == InventoryOperationType.AdjustDecrease
            || operationType == InventoryOperationType.TransferOut
            || operationType == InventoryOperationType.BackflushDecrease)
        {
            Quantity -= changeQuantity;
            if (!allowNegativeInventory && Quantity < 0m)
            {
                throw new BusinessException("WMS:Inventory:NegativeQuantity",
                    $"Negative inventory not allowed. Material {MaterialCode}, Warehouse {WarehouseCode}, " +
                    $"Quantity would be {Quantity} after deducting {changeQuantity}.");
            }
        }
        else if (operationType == InventoryOperationType.Freeze)
        {
            FrozenQuantity += changeQuantity;
        }
        else if (operationType == InventoryOperationType.Unfreeze)
        {
            FrozenQuantity -= changeQuantity;
            if (FrozenQuantity < 0m)
            {
                throw new BusinessException("WMS:Inventory:UnfreezeExceedsFrozen",
                    $"Unfreeze quantity exceeds frozen quantity. Material {MaterialCode}, " +
                    $"FrozenQuantity={FrozenQuantity + changeQuantity}, UnfreezeQty={changeQuantity}.");
            }
        }
        else
        {
            throw new BusinessException("WMS:Inventory:UnsupportedOperationType",
                $"Unsupported operation type: {operationType.Name}");
        }

        // Recalculate AvailableQuantity: Quantity - ReservedQuantity - FrozenQuantity
        AvailableQuantity = Quantity - ReservedQuantity - FrozenQuantity;
        LastOperationTime = DateTime.UtcNow;
        ConcurrencyVersion++;

        // Create ledger entry (will be persisted by DomainService)
        var ledgerEntry = new InventoryLedgerEntry(
            Guid.NewGuid(),
            Id,
            operationType,
            changeQuantity,
            beforeQuantity,
            Quantity,
            beforeAvailable,
            AvailableQuantity,
            DateTime.UtcNow,
            GetCurrentOperatorId(),
            GetCurrentOperatorName(),
            sourceOrderType,
            sourceOrderId,
            sourceOrderNo ?? string.Empty,
            null
        );

        // Publish domain event (DE-001)
        AddLocalEvent(new InventoryChangedEvent
        {
            AggregateRootId = Id,
            BalanceId = Id,
            MaterialId = MaterialId,
            WarehouseId = WarehouseId,
            ChangeQuantity = changeQuantity,
            BeforeQuantity = beforeQuantity,
            AfterQuantity = Quantity,
            OperationTypeValue = operationType.Value,
            SourceModule = "Inventory"
        });

        return new InventoryChangeResult
        {
            BalanceId = Id,
            LedgerEntry = ledgerEntry,
            ChangeQuantity = changeQuantity,
            BeforeQuantity = beforeQuantity,
            AfterQuantity = Quantity
        };
    }

    /// <summary>
    /// Reserve quantity — increases ReservedQuantity for allocation.
    /// Validates that available quantity covers the reservation.
    /// </summary>
    public void ReserveQuantity(decimal quantity, string sourceOrderType, Guid sourceOrderId)
    {
        if (quantity <= 0m)
        {
            throw new BusinessException("WMS:Inventory:InvalidReserveQuantity",
                "Reserve quantity must be positive.");
        }

        if (AvailableQuantity - quantity < 0m)
        {
            throw new BusinessException("WMS:Inventory:InsufficientAvailable",
                $"Insufficient available quantity. Available={AvailableQuantity}, " +
                $"RequestedReserve={quantity}. Material {MaterialCode}, Warehouse {WarehouseCode}.");
        }

        ReservedQuantity += quantity;
        AvailableQuantity = Quantity - ReservedQuantity - FrozenQuantity;
        LastOperationTime = DateTime.UtcNow;
        ConcurrencyVersion++;
    }

    /// <summary>
    /// Release reservation — decreases ReservedQuantity when reservation is cancelled or shipped.
    /// </summary>
    public void ReleaseReservation(decimal quantity, string sourceOrderType, Guid sourceOrderId)
    {
        if (quantity <= 0m)
        {
            throw new BusinessException("WMS:Inventory:InvalidReleaseQuantity",
                "Release quantity must be positive.");
        }

        if (ReservedQuantity - quantity < 0m)
        {
            throw new BusinessException("WMS:Inventory:ReleaseExceedsReserved",
                $"Release quantity exceeds reserved. ReservedQuantity={ReservedQuantity}, " +
                $"ReleaseQty={quantity}. Material {MaterialCode}.");
        }

        ReservedQuantity -= quantity;
        AvailableQuantity = Quantity - ReservedQuantity - FrozenQuantity;
        LastOperationTime = DateTime.UtcNow;
        ConcurrencyVersion++;
    }

    /// <summary>
    /// Freeze quantity — increases FrozenQuantity, reduces AvailableQuantity.
    /// Called by FreezeDomainService during batch freeze operations.
    /// </summary>
    public void FreezeQuantity(decimal quantity, string sourceOrderType, Guid sourceOrderId)
    {
        if (quantity <= 0m)
        {
            throw new BusinessException("WMS:Inventory:InvalidFreezeQuantity",
                "Freeze quantity must be positive.");
        }

        if (AvailableQuantity - quantity < 0m)
        {
            throw new BusinessException("WMS:Inventory:InsufficientAvailableForFreeze",
                $"Insufficient available quantity for freeze. Available={AvailableQuantity}, " +
                $"FreezeQty={quantity}. Material {MaterialCode}.");
        }

        FrozenQuantity += quantity;
        AvailableQuantity = Quantity - ReservedQuantity - FrozenQuantity;
        LastOperationTime = DateTime.UtcNow;
        ConcurrencyVersion++;
    }

    /// <summary>
    /// Unfreeze quantity — decreases FrozenQuantity, increases AvailableQuantity.
    /// Called by FreezeDomainService during batch unfreeze operations.
    /// </summary>
    public void UnfreezeQuantity(decimal quantity, string sourceOrderType, Guid sourceOrderId)
    {
        if (quantity <= 0m)
        {
            throw new BusinessException("WMS:Inventory:InvalidUnfreezeQuantity",
                "Unfreeze quantity must be positive.");
        }

        if (FrozenQuantity - quantity < 0m)
        {
            throw new BusinessException("WMS:Inventory:UnfreezeExceedsFrozen",
                $"Unfreeze quantity exceeds frozen. FrozenQuantity={FrozenQuantity}, " +
                $"UnfreezeQty={quantity}. Material {MaterialCode}.");
        }

        FrozenQuantity -= quantity;
        AvailableQuantity = Quantity - ReservedQuantity - FrozenQuantity;
        LastOperationTime = DateTime.UtcNow;
        ConcurrencyVersion++;
    }

    /// <summary>
    /// Change inventory status — transitions the status following the state machine rules (SM-04).
    /// </summary>
    public void ChangeStatus(InventoryStatus newStatus)
    {
        var stateMachine = new InventoryStatusStateMachine();
        if (!stateMachine.CanTransition(InventoryStatus, newStatus))
        {
            throw new BusinessException("WMS:Inventory:InvalidStatusTransition",
                $"Cannot transition inventory status from {InventoryStatus.Name} to {newStatus.Name}. " +
                $"Material {MaterialCode}, Warehouse {WarehouseCode}.");
        }

        InventoryStatus = newStatus;
        LastOperationTime = DateTime.UtcNow;
        ConcurrencyVersion++;
    }

    /// <summary>
    /// Update expiry information — production date and expiry date.
    /// </summary>
    public void UpdateExpiryInfo(DateTime? expiryDate, DateTime? productionDate)
    {
        ExpiryDate = expiryDate;
        ProductionDate = productionDate;
    }

    /// <summary>
    /// Update cost information — unit cost, supplier details.
    /// </summary>
    public void UpdateCost(decimal? unitCost, Guid? supplierId, string? supplierName)
    {
        UnitCost = unitCost;
        SupplierId = supplierId;
        SupplierName = supplierName;
    }

    // Helper methods for operator context (would be set from ABP ICurrentUser in AppService layer)
    private Guid GetCurrentOperatorId() => Guid.Empty; // Set by DomainService from ICurrentUser
    private string GetCurrentOperatorName() => string.Empty; // Set by DomainService from ICurrentUser
}
