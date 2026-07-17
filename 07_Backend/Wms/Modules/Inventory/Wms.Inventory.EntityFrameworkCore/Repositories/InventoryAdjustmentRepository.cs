using Microsoft.EntityFrameworkCore;
using Wms.Inventory.Domain.Aggregates;
using Wms.Inventory.Domain.Enums;
using Wms.Inventory.Domain.Repositories;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Wms.Inventory.EntityFrameworkCore.Repositories;

/// <summary>
/// Inventory Adjustment Repository Implementation.
/// </summary>
public class InventoryAdjustmentRepository : EfCoreRepository<WmsInventoryDbContext, InventoryAdjustment, Guid>,
    IInventoryAdjustmentRepository
{
    public InventoryAdjustmentRepository(IDbContextProvider<WmsInventoryDbContext> dbContextProvider)
        : base(dbContextProvider) { }

    public async Task<InventoryAdjustment?> FindByNoAsync(string adjustmentNo)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet.FirstOrDefaultAsync(a => a.AdjustmentNo == adjustmentNo);
    }

    public async Task<List<InventoryAdjustment>> GetByStatusAsync(AdjustmentApprovalStatus status, int maxResultCount = 100, int skipCount = 0)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet.Where(a => a.ApprovalStatus == status)
            .OrderByDescending(a => a.CreationTime)
            .Skip(skipCount).Take(maxResultCount).ToListAsync();
    }

    public async Task<List<InventoryAdjustment>> GetByWarehouseAsync(Guid warehouseId, int maxResultCount = 100, int skipCount = 0)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet.Where(a => a.WarehouseId == warehouseId)
            .OrderByDescending(a => a.CreationTime)
            .Skip(skipCount).Take(maxResultCount).ToListAsync();
    }

    public override async Task<IQueryable<InventoryAdjustment>> WithDetailsAsync()
    {
        return (await GetQueryableAsync()).Include(a => a.Lines);
    }
}
