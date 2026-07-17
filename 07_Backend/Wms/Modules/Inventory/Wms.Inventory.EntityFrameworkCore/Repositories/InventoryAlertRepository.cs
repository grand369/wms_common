using Microsoft.EntityFrameworkCore;
using Wms.Inventory.Domain.Aggregates;
using Wms.Inventory.Domain.Enums;
using Wms.Inventory.Domain.Repositories;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Wms.Inventory.EntityFrameworkCore.Repositories;

/// <summary>
/// Inventory Alert Repository Implementation.
/// </summary>
public class InventoryAlertRepository : EfCoreRepository<WmsInventoryDbContext, InventoryAlert, Guid>,
    IInventoryAlertRepository
{
    public InventoryAlertRepository(IDbContextProvider<WmsInventoryDbContext> dbContextProvider)
        : base(dbContextProvider) { }

    public async Task<List<InventoryAlert>> GetActiveAlertsAsync(int maxResultCount = 100, int skipCount = 0)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet.Where(a => !a.IsResolved)
            .OrderByDescending(a => a.AlertTime)
            .Skip(skipCount).Take(maxResultCount).ToListAsync();
    }

    public async Task<List<InventoryAlert>> GetByTypeAsync(AlertType alertType, int maxResultCount = 100, int skipCount = 0)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet.Where(a => a.AlertType == alertType)
            .OrderByDescending(a => a.AlertTime)
            .Skip(skipCount).Take(maxResultCount).ToListAsync();
    }

    public async Task<List<InventoryAlert>> GetByMaterialAsync(Guid materialId, int maxResultCount = 100, int skipCount = 0)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet.Where(a => a.MaterialId == materialId)
            .OrderByDescending(a => a.AlertTime)
            .Skip(skipCount).Take(maxResultCount).ToListAsync();
    }
}
