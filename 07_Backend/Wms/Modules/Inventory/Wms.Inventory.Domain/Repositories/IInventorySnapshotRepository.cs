using Volo.Abp.Domain.Repositories;
using Wms.Inventory.Domain.Aggregates;

namespace Wms.Inventory.Domain.Repositories;

/// <summary>
/// Inventory Snapshot Repository Interface — supports querying and persisting snapshots.
/// </summary>
public interface IInventorySnapshotRepository : IRepository<InventorySnapshot, Guid>
{
    Task<List<InventorySnapshot>> GetByWarehouseIdAsync(Guid warehouseId);
    Task<InventorySnapshot?> GetLatestByWarehouseIdAsync(Guid warehouseId);
}
