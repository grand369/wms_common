using Microsoft.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using Wms.Warehouse.Domain.Aggregates;
using Wms.Warehouse.Domain.Repositories;

namespace Wms.Warehouse.EntityFrameworkCore.Repositories;

/// <summary>
/// Location Repository — implements ILocationRepository using EF Core.
/// (Phase 3 DDD Design, Section 7)
/// </summary>
public class LocationRepository : EfCoreRepository<WmsWarehouseDbContext, Location, Guid>, ILocationRepository
{
    public LocationRepository(IDbContextProvider<WmsWarehouseDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }

    public async Task<Location?> FindByCodeAsync(string locationCode)
    {
        var dbContext = await GetDbContextAsync();
        return await dbContext.Locations
            .FirstOrDefaultAsync(l => l.LocationCode == locationCode);
    }

    public async Task<Location?> FindByBarcodeIdAsync(string barcodeId)
    {
        var dbContext = await GetDbContextAsync();
        return await dbContext.Locations
            .FirstOrDefaultAsync(l => l.BarcodeId == barcodeId);
    }

    public async Task<List<Location>> GetListByWarehouseIdAsync(string warehouseId)
    {
        var dbContext = await GetDbContextAsync();
        return await dbContext.Locations
            .Where(l => l.WarehouseId == warehouseId)
            .OrderBy(l => l.LocationCode)
            .ToListAsync();
    }

    public async Task<List<Location>> GetListByAreaIdAsync(string areaId)
    {
        var dbContext = await GetDbContextAsync();
        return await dbContext.Locations
            .Where(l => l.AreaId == areaId)
            .OrderBy(l => l.LocationCode)
            .ToListAsync();
    }

    public async Task<List<Location>> GetAvailableLocationsAsync(string warehouseId, int? storageCondition = null)
    {
        var dbContext = await GetDbContextAsync();
        var query = dbContext.Locations
            .Where(l => l.WarehouseId == warehouseId && l.IsActive);

        if (storageCondition.HasValue)
        {
            query = query.Where(l => l.StorageCondition == storageCondition.Value);
        }

        return await query
            .OrderBy(l => l.LocationCode)
            .ToListAsync();
    }

    public async Task<bool> CodeExistsAsync(string locationCode)
    {
        var dbContext = await GetDbContextAsync();
        return await dbContext.Locations
            .AnyAsync(l => l.LocationCode == locationCode);
    }
}
