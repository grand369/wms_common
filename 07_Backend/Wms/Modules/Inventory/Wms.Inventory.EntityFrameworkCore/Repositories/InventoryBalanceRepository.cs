using Microsoft.EntityFrameworkCore;
using Wms.Inventory.Domain.Aggregates;
using Wms.Inventory.Domain.Enums;
using Wms.Inventory.Domain.Repositories;
using Wms.Shared.Domain.Enums;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Wms.Inventory.EntityFrameworkCore.Repositories;

/// <summary>
/// Inventory Balance Repository Implementation — implements all custom query methods.
/// Core repository for the entire platform.
/// </summary>
public class InventoryBalanceRepository : EfCoreRepository<WmsInventoryDbContext, InventoryBalance, Guid>,
    IInventoryBalanceRepository
{
    public InventoryBalanceRepository(IDbContextProvider<WmsInventoryDbContext> dbContextProvider)
        : base(dbContextProvider) { }

    public async Task<InventoryBalance?> FindAsync(
        Guid materialId, Guid warehouseId, Guid locationId,
        string? batchNumber, InventoryStatus status)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet.FirstOrDefaultAsync(b =>
            b.MaterialId == materialId &&
            b.WarehouseId == warehouseId &&
            b.LocationId == locationId &&
            b.BatchNumber == batchNumber &&
            b.InventoryStatus == status);
    }

    public async Task<List<InventoryBalance>> GetByWarehouseAsync(Guid warehouseId, int maxResultCount = 1000)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet.Where(b => b.WarehouseId == warehouseId)
            .Take(maxResultCount).ToListAsync();
    }

    public async Task<List<InventoryBalance>> GetByMaterialAsync(Guid materialId, int maxResultCount = 1000)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet.Where(b => b.MaterialId == materialId)
            .Take(maxResultCount).ToListAsync();
    }

    public async Task<List<InventoryBalance>> GetByLocationAsync(Guid locationId, int maxResultCount = 1000)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet.Where(b => b.LocationId == locationId)
            .Take(maxResultCount).ToListAsync();
    }

    public async Task<List<InventoryBalance>> GetByBatchAsync(string batchNumber, int maxResultCount = 1000)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet.Where(b => b.BatchNumber == batchNumber)
            .Take(maxResultCount).ToListAsync();
    }

    public async Task<List<InventoryBalance>> GetByStatusAsync(InventoryStatus status, int maxResultCount = 1000)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet.Where(b => b.InventoryStatus == status)
            .Take(maxResultCount).ToListAsync();
    }

    public async Task<List<InventoryBalance>> GetAvailableForPickingAsync(
        Guid materialId, Guid warehouseId, string strategyType = "FIFO")
    {
        var dbSet = await GetDbSetAsync();
        var query = dbSet.Where(b =>
            b.MaterialId == materialId &&
            b.WarehouseId == warehouseId &&
            b.InventoryStatus == InventoryStatus.Available &&
            b.AvailableQuantity > 0);

        // Apply picking strategy
        query = strategyType == "FEFO"
            ? query.OrderBy(b => b.ExpiryDate ?? DateTime.MaxValue)
            : strategyType == "FMFO"
                ? query.OrderByDescending(b => b.LastOperationTime)
                : query.OrderBy(b => b.LastOperationTime); // FIFO = earliest first

        return await query.ToListAsync();
    }

    public async Task<List<InventoryBalance>> GetBelowSafetyStockAsync()
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet.Where(b =>
            b.InventoryStatus == InventoryStatus.Available &&
            b.AvailableQuantity <= 0).ToListAsync();
    }

    public async Task<List<InventoryBalance>> GetNearExpiryAsync(int alertDays = 30)
    {
        var dbSet = await GetDbSetAsync();
        var thresholdDate = DateTime.UtcNow.AddDays(alertDays);
        return await dbSet.Where(b =>
            b.ExpiryDate.HasValue &&
            b.ExpiryDate.Value <= thresholdDate &&
            b.InventoryStatus == InventoryStatus.Available).ToListAsync();
    }

    public async Task<List<InventoryBalance>> GetZeroInventoryAsync()
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet.Where(b => b.Quantity == 0).ToListAsync();
    }
}
