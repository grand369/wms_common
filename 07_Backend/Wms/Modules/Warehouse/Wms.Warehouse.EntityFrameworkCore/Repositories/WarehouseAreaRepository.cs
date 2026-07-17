using Microsoft.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using Wms.Warehouse.Domain.Aggregates;
using Wms.Warehouse.Domain.Repositories;

namespace Wms.Warehouse.EntityFrameworkCore.Repositories;

/// <summary>
/// Warehouse Area Repository — implements IWarehouseAreaRepository using EF Core.
/// (Phase 3 DDD Design, Section 7)
/// </summary>
public class WarehouseAreaRepository : EfCoreRepository<WmsWarehouseDbContext, WarehouseArea, Guid>, IWarehouseAreaRepository
{
    public WarehouseAreaRepository(IDbContextProvider<WmsWarehouseDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }

    public async Task<WarehouseArea?> FindByCodeAndWarehouseIdAsync(string areaCode, string warehouseId)
    {
        var dbContext = await GetDbContextAsync();
        return await dbContext.WarehouseAreas
            .FirstOrDefaultAsync(a => a.AreaCode == areaCode && a.WarehouseId == warehouseId);
    }

    public async Task<List<WarehouseArea>> GetListByWarehouseIdAsync(string warehouseId)
    {
        var dbContext = await GetDbContextAsync();
        return await dbContext.WarehouseAreas
            .Where(a => a.WarehouseId == warehouseId)
            .OrderBy(a => a.AreaCode)
            .ToListAsync();
    }

    public async Task<bool> CodeExistsInWarehouseAsync(string areaCode, string warehouseId)
    {
        var dbContext = await GetDbContextAsync();
        return await dbContext.WarehouseAreas
            .AnyAsync(a => a.AreaCode == areaCode && a.WarehouseId == warehouseId);
    }
}
