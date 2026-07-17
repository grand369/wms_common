using Microsoft.EntityFrameworkCore;
using Wms.Inventory.Domain.Aggregates;
using Wms.Inventory.Domain.Enums;
using Wms.Inventory.Domain.Repositories;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Wms.Inventory.EntityFrameworkCore.Repositories;

/// <summary>
/// Inventory Ledger Repository Implementation — ⚠️ CRITICAL immutable repository.
/// Update/Delete methods throw NotSupportedException (BR-010).
/// </summary>
public class InventoryLedgerRepository : EfCoreRepository<WmsInventoryDbContext, InventoryLedgerEntry, Guid>,
    IInventoryLedgerRepository
{
    public InventoryLedgerRepository(IDbContextProvider<WmsInventoryDbContext> dbContextProvider)
        : base(dbContextProvider) { }

    /// <summary>⚠️ NotSupportedException — ledger entries cannot be updated (BR-010).</summary>
    public override Task<InventoryLedgerEntry> UpdateAsync(InventoryLedgerEntry entity, bool autoSave = false, CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException("Inventory ledger entries cannot be updated (BR-010).");
    }

    /// <summary>⚠️ NotSupportedException — ledger entries cannot be deleted (BR-010).</summary>
    public override Task DeleteAsync(InventoryLedgerEntry entity, bool autoSave = false, CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException("Inventory ledger entries cannot be deleted (BR-010).");
    }

    /// <summary>⚠️ NotSupportedException — ledger entries cannot be deleted by ID (BR-010).</summary>
    public override Task DeleteAsync(Guid id, bool autoSave = false, CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException("Inventory ledger entries cannot be deleted (BR-010).");
    }

    public async Task<List<InventoryLedgerEntry>> GetByBalanceIdAsync(Guid inventoryBalanceId, int maxResultCount = 100, int skipCount = 0)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet.Where(l => l.InventoryBalanceId == inventoryBalanceId)
            .OrderByDescending(l => l.OperationTime)
            .Skip(skipCount).Take(maxResultCount).ToListAsync();
    }

    public async Task<List<InventoryLedgerEntry>> GetBySourceOrderAsync(string sourceOrderType, Guid sourceOrderId)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet.Where(l =>
            l.SourceOrderType == sourceOrderType &&
            l.SourceOrderId == sourceOrderId)
            .OrderByDescending(l => l.OperationTime).ToListAsync();
    }

    public async Task<List<InventoryLedgerEntry>> GetByTimeRangeAsync(DateTime startTime, DateTime endTime, int maxResultCount = 100, int skipCount = 0)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet.Where(l =>
            l.OperationTime >= startTime &&
            l.OperationTime <= endTime)
            .OrderByDescending(l => l.OperationTime)
            .Skip(skipCount).Take(maxResultCount).ToListAsync();
    }

    public async Task<List<InventoryLedgerEntry>> GetByMaterialAsync(Guid materialId, DateTime? startTime = null, DateTime? endTime = null, int maxResultCount = 100, int skipCount = 0)
    {
        // Note: InventoryLedgerEntry doesn't have MaterialId directly —
        // we'd need to join with InventoryBalance. Simplified for v1.0:
        var dbContext = await GetDbContextAsync();
        return await dbContext.InventoryLedgerEntries
            .Join(dbContext.InventoryBalances,
                l => l.InventoryBalanceId,
                b => b.Id,
                (l, b) => new { Ledger = l, Balance = b })
            .Where(j => j.Balance.MaterialId == materialId)
            .Where(j => !startTime.HasValue || j.Ledger.OperationTime >= startTime.Value)
            .Where(j => !endTime.HasValue || j.Ledger.OperationTime <= endTime.Value)
            .OrderByDescending(j => j.Ledger.OperationTime)
            .Select(j => j.Ledger)
            .Skip(skipCount).Take(maxResultCount).ToListAsync();
    }
}
