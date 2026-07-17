using Wms.Inventory.Domain.Aggregates;
using Wms.Inventory.Domain.Enums;

namespace Wms.Inventory.Domain.Repositories;

/// <summary>
/// Inventory Adjustment Repository Interface — specialized queries for adjustment records.
/// </summary>
public interface IInventoryAdjustmentRepository : IRepository<InventoryAdjustment, Guid>
{
    /// <summary>Find by adjustment number (business natural key).</summary>
    Task<InventoryAdjustment?> FindByNoAsync(string adjustmentNo);

    /// <summary>Get adjustments by approval status.</summary>
    Task<List<InventoryAdjustment>> GetByStatusAsync(AdjustmentApprovalStatus status, int maxResultCount = 100, int skipCount = 0);

    /// <summary>Get adjustments for a warehouse.</summary>
    Task<List<InventoryAdjustment>> GetByWarehouseAsync(Guid warehouseId, int maxResultCount = 100, int skipCount = 0);
}
