using Wms.Inventory.Domain.Aggregates;
using Wms.Inventory.Domain.Enums;
using Wms.Shared.Domain.Enums;

namespace Wms.Inventory.Domain.Repositories;

/// <summary>
/// Inventory Balance Repository Interface (REP-06) — core repository for the platform's heart.
/// Provides specialized query methods beyond standard CRUD.
/// </summary>
public interface IInventoryBalanceRepository : IRepository<InventoryBalance, Guid>
{
    /// <summary>Find by composite unique key (MaterialId, WarehouseId, LocationId, BatchNumber, InventoryStatus).</summary>
    Task<InventoryBalance?> FindAsync(
        Guid materialId,
        Guid warehouseId,
        Guid locationId,
        string? batchNumber,
        InventoryStatus status);

    /// <summary>Get all balances in a warehouse.</summary>
    Task<List<InventoryBalance>> GetByWarehouseAsync(Guid warehouseId, int maxResultCount = 1000);

    /// <summary>Get all balances for a material across all warehouses.</summary>
    Task<List<InventoryBalance>> GetByMaterialAsync(Guid materialId, int maxResultCount = 1000);

    /// <summary>Get all balances at a specific location.</summary>
    Task<List<InventoryBalance>> GetByLocationAsync(Guid locationId, int maxResultCount = 1000);

    /// <summary>Get all balances for a specific batch number.</summary>
    Task<List<InventoryBalance>> GetByBatchAsync(string batchNumber, int maxResultCount = 1000);

    /// <summary>Get all balances with a specific status.</summary>
    Task<List<InventoryBalance>> GetByStatusAsync(InventoryStatus status, int maxResultCount = 1000);

    /// <summary>Get available inventory for picking — sorted by issue strategy (FIFO/FEFO/etc).</summary>
    Task<List<InventoryBalance>> GetAvailableForPickingAsync(
        Guid materialId,
        Guid warehouseId,
        string strategyType = "FIFO");

    /// <summary>Scan for balances below safety stock threshold.</summary>
    Task<List<InventoryBalance>> GetBelowSafetyStockAsync();

    /// <summary>Scan for balances near expiry date.</summary>
    Task<List<InventoryBalance>> GetNearExpiryAsync(int alertDays = 30);

    /// <summary>Scan for zero inventory balances.</summary>
    Task<List<InventoryBalance>> GetZeroInventoryAsync();
}
