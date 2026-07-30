using Microsoft.EntityFrameworkCore;
using Wms.Inventory.Domain.Aggregates;
using Wms.Inventory.Domain.Repositories;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Wms.Inventory.EntityFrameworkCore.Repositories;

/// <summary>
/// Inventory Snapshot Repository Implementation.
/// </summary>
public class InventorySnapshotRepository : EfCoreRepository<WmsInventoryDbContext, InventorySnapshot, Guid>,
    IInventorySnapshotRepository
{
    public InventorySnapshotRepository(IDbContextProvider<WmsInventoryDbContext> dbContextProvider)
        : base(dbContextProvider) { }

    public async Task<List<InventorySnapshot>> GetByWarehouseIdAsync(Guid warehouseId)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet.Where(s => s.WarehouseId == warehouseId)
            .OrderByDescending(s => s.SnapshotTime)
            .ToListAsync();
    }

    public async Task<InventorySnapshot?> GetLatestByWarehouseIdAsync(Guid warehouseId)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet.Where(s => s.WarehouseId == warehouseId)
            .OrderByDescending(s => s.SnapshotTime)
            .FirstOrDefaultAsync();
    }
}
