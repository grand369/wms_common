using Microsoft.EntityFrameworkCore;
using Wms.Inventory.Domain.Aggregates;
using Wms.Inventory.Domain.Enums;
using Wms.Inventory.Domain.Repositories;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Wms.Inventory.EntityFrameworkCore.Repositories;

/// <summary>
/// Inventory Freeze Order Repository Implementation.
/// </summary>
public class InventoryFreezeOrderRepository : EfCoreRepository<WmsInventoryDbContext, InventoryFreezeOrder, Guid>,
    IInventoryFreezeOrderRepository
{
    public InventoryFreezeOrderRepository(IDbContextProvider<WmsInventoryDbContext> dbContextProvider)
        : base(dbContextProvider) { }

    public async Task<InventoryFreezeOrder?> FindByNoAsync(string freezeOrderNo)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet.FirstOrDefaultAsync(f => f.FreezeOrderNo == freezeOrderNo);
    }

    public async Task<List<InventoryFreezeOrder>> GetByStatusAsync(FreezeStatus status, int maxResultCount = 100, int skipCount = 0)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet.Where(f => f.FreezeStatus == status)
            .OrderByDescending(f => f.CreationTime)
            .Skip(skipCount).Take(maxResultCount).ToListAsync();
    }

    public async Task<List<InventoryFreezeOrder>> GetByWarehouseAsync(Guid warehouseId, int maxResultCount = 100, int skipCount = 0)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet.Where(f => f.WarehouseId == warehouseId)
            .OrderByDescending(f => f.CreationTime)
            .Skip(skipCount).Take(maxResultCount).ToListAsync();
    }
}
