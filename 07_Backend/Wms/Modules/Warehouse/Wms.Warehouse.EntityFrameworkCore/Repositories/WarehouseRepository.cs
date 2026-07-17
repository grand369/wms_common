using Microsoft.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using Wms.Warehouse.Domain.Aggregates;
using WarehouseEntity = Wms.Warehouse.Domain.Aggregates.Warehouse;
using Wms.Warehouse.Domain.Repositories;

namespace Wms.Warehouse.EntityFrameworkCore.Repositories;

/// <summary>
/// Warehouse Repository — implements IWarehouseRepository using EF Core.
/// (Phase 3 DDD Design, Section 7)
/// </summary>
public class WarehouseRepository : EfCoreRepository<WmsWarehouseDbContext, WarehouseEntity, Guid>, IWarehouseRepository
{
    public WarehouseRepository(IDbContextProvider<WmsWarehouseDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }

    public async Task<WarehouseEntity?> FindByCodeAsync(string warehouseCode)
    {
        var dbContext = await GetDbContextAsync();
        return await dbContext.Warehouses
            .FirstOrDefaultAsync(w => w.WarehouseCode == warehouseCode);
    }

    public async Task<List<WarehouseEntity>> GetListByOrganizationIdAsync(string organizationUnitId)
    {
        var dbContext = await GetDbContextAsync();
        return await dbContext.Warehouses
            .Where(w => w.OrganizationUnitId == organizationUnitId)
            .OrderBy(w => w.WarehouseCode)
            .ToListAsync();
    }

    public async Task<List<WarehouseEntity>> GetActiveListAsync()
    {
        var dbContext = await GetDbContextAsync();
        return await dbContext.Warehouses
            .Where(w => w.IsActive)
            .OrderBy(w => w.WarehouseCode)
            .ToListAsync();
    }

    public async Task<bool> CodeExistsAsync(string warehouseCode)
    {
        var dbContext = await GetDbContextAsync();
        return await dbContext.Warehouses
            .AnyAsync(w => w.WarehouseCode == warehouseCode);
    }
}
