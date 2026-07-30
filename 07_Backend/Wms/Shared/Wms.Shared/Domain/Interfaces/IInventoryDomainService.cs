namespace Wms.Shared.Domain.Interfaces;

/// <summary>
/// IInventoryDomainService — cross-module interface for inventory domain operations.
/// Defined in Shared Kernel so that Inbound, Outbound, and other modules can
/// inject this interface via DI to call Inventory domain methods synchronously
/// within their own UoW transaction (CROSS-002, Phase 6 API Design).
///
/// ⚠️ Return types use simple Guid instead of InventoryChangeResult to avoid
/// Contracts depending on Inventory.Domain types. The caller can look up the
/// balance by ID if needed.
/// </summary>
public interface IInventoryDomainService
{
    /// <summary>
    /// Increase inventory — find or create balance, apply quantity change, write ledger.
    /// Used by Inbound module for receipt completion (synchronous, same UoW).
    /// </summary>
    Task<Guid> IncreaseInventoryAsync(
        Guid materialId, Guid warehouseId, Guid locationId, string? batchNo,
        decimal qty, string materialCode, string warehouseCode, string locationCode,
        string srcType, Guid srcId, bool allowNegative = false);

    /// <summary>
    /// Decrease inventory — find balance, apply quantity decrease, write ledger.
    /// Used by Outbound module for shipment completion (synchronous, same UoW).
    /// </summary>
    Task<Guid> DecreaseInventoryAsync(
        Guid materialId, Guid warehouseId, Guid locationId, string? batchNo,
        int inventoryStatusValue, decimal qty,
        string srcType, Guid srcId, bool allowNegative = false);

    /// <summary>
    /// Reserve inventory — find balance, reserve quantity for allocation.
    /// Used by Outbound module for allocation (synchronous, same UoW).
    /// </summary>
    Task ReserveInventoryAsync(
        Guid materialId, Guid warehouseId, Guid locationId, string? batchNo,
        int inventoryStatusValue, decimal reqQty, string srcOrderType, Guid srcOrderId);

    /// <summary>
    /// Release reservation — release previously reserved quantity.
    /// Used by Outbound module for cancellation or after shipment (synchronous, same UoW).
    /// </summary>
    Task ReleaseReservationAsync(
        Guid materialId, Guid warehouseId, Guid locationId, string? batchNo,
        int inventoryStatusValue, decimal qty, string srcOrderType, Guid srcOrderId);

    /// <summary>
    /// Find available balances for picking — returns balances sorted by issue strategy.
    /// Used by Outbound module to auto-allocate inventory based on FIFO/FEFO/FMFO strategy.
    /// </summary>
    Task<List<AvailableBalanceInfo>> FindAvailableBalancesAsync(
        Guid materialId, Guid warehouseId, string strategyType = "FIFO");
}

/// <summary>
/// AvailableBalanceInfo — simplified DTO for cross-module available balance lookup.
/// </summary>
public class AvailableBalanceInfo
{
    public Guid BalanceId { get; set; }
    public Guid LocationId { get; set; }
    public string LocationCode { get; set; } = string.Empty;
    public decimal AvailableQuantity { get; set; }
    public string? BatchNumber { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public DateTime? ProductionDate { get; set; }
    public DateTime CreationTime { get; set; }
}
