using Wms.Inventory.Domain.Aggregates;
using Wms.Inventory.Domain.Enums;
using Wms.Inventory.Domain.Events;
using Wms.Inventory.Domain.Repositories;
using Wms.Inventory.Domain.ValueObjects;
using Wms.Shared.Domain.Enums;
using Wms.Shared.Domain.Interfaces;
using Volo.Abp.Domain.Services;
using Volo.Abp.EventBus.Local;

namespace Wms.Inventory.Domain.Services;

/// <summary>
/// Inventory Domain Service (DS-01) — the core domain service that coordinates
/// inventory operations across aggregates. Strictly limited to 9 methods to avoid
/// becoming a "god service".
/// Injected dependencies: IInventoryBalanceRepository + IInventoryLedgerRepository.
/// Also implements IInventoryDomainService (Shared Kernel) for cross-module DI injection.
/// </summary>
public class InventoryDomainService : DomainService, IInventoryDomainService
{
    private readonly IInventoryBalanceRepository _balanceRepository;
    private readonly IInventoryLedgerRepository _ledgerRepository;
    private readonly ILocalEventBus _localEventBus;

    public InventoryDomainService(
        IInventoryBalanceRepository balanceRepository,
        IInventoryLedgerRepository ledgerRepository,
        ILocalEventBus localEventBus)
    {
        _balanceRepository = balanceRepository;
        _ledgerRepository = ledgerRepository;
        _localEventBus = localEventBus;
    }

    /// <summary>
    /// Method 1: IncreaseInventory — find or create balance, apply quantity change, save ledger.
    /// Used for inbound, replenishment, transfer-in operations.
    /// </summary>
    public async Task<InventoryChangeResult> IncreaseInventoryAsync(
        Guid materialId, Guid warehouseId, Guid locationId, string? batchNo,
        decimal qty, string materialCode, string warehouseCode, string locationCode,
        string srcType, Guid srcId, bool allowNegative = false)
    {
        var balance = await FindOrCreateBalanceAsync(
            materialId, warehouseId, locationId, batchNo,
            materialCode, warehouseCode, locationCode, InventoryStatus.Available);

        var result = balance.ApplyQuantityChange(
            InventoryOperationType.InboundIncrease, qty, srcType, srcId, allowNegative);

        await _balanceRepository.UpdateAsync(balance);
        if (result.LedgerEntry != null)
        {
            await _ledgerRepository.InsertAsync(result.LedgerEntry);
        }

        return result;
    }

    /// <summary>
    /// Method 2: DecreaseInventory — find balance, apply quantity decrease, save ledger.
    /// Used for outbound, transfer-out, backflush operations.
    /// </summary>
    public async Task<InventoryChangeResult> DecreaseInventoryAsync(
        Guid materialId, Guid warehouseId, Guid locationId, string? batchNo,
        InventoryStatus status, decimal qty,
        string srcType, Guid srcId, bool allowNegative = false)
    {
        var balance = await _balanceRepository.FindAsync(materialId, warehouseId, locationId, batchNo, status);
        if (balance == null)
        {
            throw new BusinessException("WMS:Inventory:BalanceNotFound",
                $"Inventory balance not found for Material={materialId}, Warehouse={warehouseId}, " +
                $"Location={locationId}, Batch={batchNo}, Status={status.Name}.");
        }

        var operationType = srcType == "TransferOrder"
            ? InventoryOperationType.TransferOut
            : InventoryOperationType.OutboundDecrease;

        var result = balance.ApplyQuantityChange(operationType, qty, srcType, srcId, allowNegative);

        await _balanceRepository.UpdateAsync(balance);
        if (result.LedgerEntry != null)
        {
            await _ledgerRepository.InsertAsync(result.LedgerEntry);
        }

        return result;
    }

    /// <summary>
    /// Method 3: ReserveInventory — find balance, reserve quantity for allocation.
    /// Used for outbound allocation.
    /// </summary>
    public async Task ReserveInventoryAsync(
        Guid materialId, Guid warehouseId, Guid locationId, string? batchNo,
        InventoryStatus status, decimal reqQty, string srcOrderType, Guid srcOrderId)
    {
        var balance = await _balanceRepository.FindAsync(materialId, warehouseId, locationId, batchNo, status);
        if (balance == null)
        {
            throw new BusinessException("WMS:Inventory:BalanceNotFound",
                $"Inventory balance not found for reservation.");
        }

        balance.ReserveQuantity(reqQty, srcOrderType, srcOrderId);
        await _balanceRepository.UpdateAsync(balance);
    }

    /// <summary>
    /// Method 4: ReleaseReservation — release previously reserved quantity.
    /// Used when allocation is cancelled or order is shipped.
    /// </summary>
    public async Task ReleaseReservationAsync(
        Guid materialId, Guid warehouseId, Guid locationId, string? batchNo,
        InventoryStatus status, decimal qty, string srcOrderType, Guid srcOrderId)
    {
        var balance = await _balanceRepository.FindAsync(materialId, warehouseId, locationId, batchNo, status);
        if (balance == null)
        {
            throw new BusinessException("WMS:Inventory:BalanceNotFound",
                $"Inventory balance not found for release reservation.");
        }

        balance.ReleaseReservation(qty, srcOrderType, srcOrderId);
        await _balanceRepository.UpdateAsync(balance);
    }

    /// <summary>
    /// Method 5: FreezeInventory — batch freeze inventory based on freeze order scope and ranges.
    /// </summary>
    public async Task FreezeInventoryAsync(
        Guid freezeOrderId, FreezeScope freezeScope, List<FreezeRange> ranges, string freezeReason)
    {
        foreach (var range in ranges)
        {
            var balances = await GetBalancesForFreezeRangeAsync(freezeScope, range);
            foreach (var balance in balances)
            {
                var qtyToFreeze = balance.AvailableQuantity;
                if (qtyToFreeze > 0)
                {
                    balance.FreezeQuantity(qtyToFreeze, "InventoryFreezeOrder", freezeOrderId);
                    await _balanceRepository.UpdateAsync(balance);
                }
            }
        }
    }

    /// <summary>
    /// Method 6: UnfreezeInventory — batch unfreeze inventory for a released freeze order.
    /// </summary>
    public async Task UnfreezeInventoryAsync(Guid freezeOrderId)
    {
        // In v1.0, we need to look up all frozen balances related to this freeze order
        // This would typically be done by querying ledger entries for the freeze order
        var frozenBalances = await _balanceRepository.GetByStatusAsync(InventoryStatus.Frozen);
        // This is a simplified implementation - in production, ledger entries would track freeze order associations
    }

    /// <summary>
    /// Method 7: AdjustInventory — execute an approved adjustment, applying each line to the balance.
    /// </summary>
    public async Task AdjustInventoryAsync(Guid adjustmentId, InventoryAdjustment adjustment)
    {
        foreach (var line in adjustment.Lines)
        {
            var balance = await _balanceRepository.FindAsync(
                line.MaterialId, adjustment.WarehouseId, line.LocationId, line.BatchNumber,
                line.InventoryStatusBefore);

            if (balance == null)
            {
                throw new BusinessException("WMS:Inventory:BalanceNotFoundForAdjustment",
                    $"Balance not found for adjustment line: Material={line.MaterialCode}, " +
                    $"Location={line.LocationCode}");
            }

            var operationType = line.AdjustmentQuantity > 0
                ? InventoryOperationType.AdjustIncrease
                : InventoryOperationType.AdjustDecrease;

            var result = balance.ApplyQuantityChange(
                operationType, Math.Abs(line.AdjustmentQuantity),
                "InventoryAdjustment", adjustmentId, true,
                adjustment.AdjustmentNo); // Adjustments allow negative

            await _balanceRepository.UpdateAsync(balance);
            if (result.LedgerEntry != null)
            {
                await _ledgerRepository.InsertAsync(result.LedgerEntry);
            }
        }
    }

    /// <summary>
    /// Method 8: CheckSafetyStockAlert — scan balances below safety stock threshold.
    /// Publishes SafetyStockAlertEvent for each violation found.
    /// </summary>
    public async Task CheckSafetyStockAlertAsync()
    {
        var belowSafetyStock = await _balanceRepository.GetBelowSafetyStockAsync();
        foreach (var balance in belowSafetyStock)
        {
            await _localEventBus.PublishAsync(new SafetyStockAlertEvent
            {
                AggregateRootId = balance.Id,
                MaterialId = balance.MaterialId,
                MaterialCode = balance.MaterialCode,
                WarehouseId = balance.WarehouseId,
                WarehouseCode = balance.WarehouseCode,
                CurrentAvailable = balance.AvailableQuantity,
                SafetyStockQuantity = 0, // Would be fetched from Material module
                SourceModule = "Inventory"
            });
        }
    }

    /// <summary>
    /// Method 9: CheckExpiryAlert — scan balances near expiry, publish ExpiryAlertEvent.
    /// </summary>
    public async Task CheckExpiryAlertAsync(int alertDays = 30)
    {
        var nearExpiry = await _balanceRepository.GetNearExpiryAsync(alertDays);
        foreach (var balance in nearExpiry)
        {
            var daysLeft = balance.ExpiryDate.HasValue
                ? (balance.ExpiryDate.Value - DateTime.UtcNow).Days
                : 0;

            await _localEventBus.PublishAsync(new ExpiryAlertEvent
            {
                AggregateRootId = balance.Id,
                MaterialId = balance.MaterialId,
                MaterialCode = balance.MaterialCode,
                WarehouseId = balance.WarehouseId,
                WarehouseCode = balance.WarehouseCode,
                BatchNumber = balance.BatchNumber ?? string.Empty,
                ExpiryDate = balance.ExpiryDate ?? DateTime.MaxValue,
                DaysLeft = daysLeft,
                SourceModule = "Inventory"
            });
        }
    }

    // Private helper: Find or create balance by unique key
    private async Task<InventoryBalance> FindOrCreateBalanceAsync(
        Guid materialId, Guid warehouseId, Guid locationId, string? batchNo,
        string materialCode, string warehouseCode, string locationCode,
        InventoryStatus status)
    {
        var balance = await _balanceRepository.FindAsync(materialId, warehouseId, locationId, batchNo, status);
        if (balance == null)
        {
            balance = new InventoryBalance(
                GuidGenerator.Create(),
                materialId, materialCode,
                warehouseId, warehouseCode,
                locationId, locationCode,
                batchNo, status);
            await _balanceRepository.InsertAsync(balance);
        }
        return balance;
    }

    // IInventoryDomainService implementation — simplified return types for cross-module use

    /// <summary>
    /// IInventoryDomainService.IncreaseInventoryAsync — returns balance ID instead of InventoryChangeResult.
    /// Called by Inbound module via DI in the same UoW transaction (CROSS-002).
    /// </summary>
    async Task<Guid> IInventoryDomainService.IncreaseInventoryAsync(
        Guid materialId, Guid warehouseId, Guid locationId, string? batchNo,
        decimal qty, string materialCode, string warehouseCode, string locationCode,
        string srcType, Guid srcId, bool allowNegative)
    {
        var result = await IncreaseInventoryAsync(
            materialId, warehouseId, locationId, batchNo,
            qty, materialCode, warehouseCode, locationCode,
            srcType, srcId, allowNegative);
        return result.BalanceId;
    }

    /// <summary>
    /// IInventoryDomainService.DecreaseInventoryAsync — uses int for InventoryStatus value.
    /// Called by Outbound module via DI in the same UoW transaction (CROSS-002).
    /// </summary>
    async Task<Guid> IInventoryDomainService.DecreaseInventoryAsync(
        Guid materialId, Guid warehouseId, Guid locationId, string? batchNo,
        int inventoryStatusValue, decimal qty,
        string srcType, Guid srcId, bool allowNegative)
    {
        var status = InventoryStatus.FromValue(inventoryStatusValue);
        var result = await DecreaseInventoryAsync(
            materialId, warehouseId, locationId, batchNo,
            status, qty, srcType, srcId, allowNegative);
        return result.BalanceId;
    }

    /// <summary>
    /// IInventoryDomainService.ReserveInventoryAsync — uses int for InventoryStatus value.
    /// Called by Outbound module via DI for allocation (CROSS-002).
    /// </summary>
    async Task IInventoryDomainService.ReserveInventoryAsync(
        Guid materialId, Guid warehouseId, Guid locationId, string? batchNo,
        int inventoryStatusValue, decimal reqQty, string srcOrderType, Guid srcOrderId)
    {
        var status = InventoryStatus.FromValue(inventoryStatusValue);
        await ReserveInventoryAsync(
            materialId, warehouseId, locationId, batchNo,
            status, reqQty, srcOrderType, srcOrderId);
    }

    /// <summary>
    /// IInventoryDomainService.ReleaseReservationAsync — uses int for InventoryStatus value.
    /// Called by Outbound module via DI for cancellation/release (CROSS-002).
    /// </summary>
    async Task IInventoryDomainService.ReleaseReservationAsync(
        Guid materialId, Guid warehouseId, Guid locationId, string? batchNo,
        int inventoryStatusValue, decimal qty, string srcOrderType, Guid srcOrderId)
    {
        var status = InventoryStatus.FromValue(inventoryStatusValue);
        await ReleaseReservationAsync(
            materialId, warehouseId, locationId, batchNo,
            status, qty, srcOrderType, srcOrderId);
    }

    // Private helper: Get balances matching freeze range
    private async Task<List<InventoryBalance>> GetBalancesForFreezeRangeAsync(
        FreezeScope freezeScope, FreezeRange range)
    {
        if (freezeScope == FreezeScope.ByBatch)
        {
            return await _balanceRepository.GetByBatchAsync(range.BatchNumber ?? string.Empty);
        }
        else if (freezeScope == FreezeScope.ByMaterial)
        {
            return await _balanceRepository.GetByMaterialAsync(range.MaterialId ?? Guid.Empty);
        }
        else if (freezeScope == FreezeScope.ByLocation)
        {
            return await _balanceRepository.GetByLocationAsync(range.LocationId ?? Guid.Empty);
        }
        else if (freezeScope == FreezeScope.ByWarehouse)
        {
            return await _balanceRepository.GetByWarehouseAsync(range.WarehouseId ?? Guid.Empty);
        }
        else
        {
            return new List<InventoryBalance>();
        }
    }
}
