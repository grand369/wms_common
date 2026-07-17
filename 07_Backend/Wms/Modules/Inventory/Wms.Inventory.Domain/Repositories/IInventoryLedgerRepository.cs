using Wms.Inventory.Domain.Aggregates;
using Wms.Inventory.Domain.Enums;

namespace Wms.Inventory.Domain.Repositories;

/// <summary>
/// Inventory Ledger Repository Interface (REP-07) — ⚠️ CRITICAL: immutable repository.
/// Update/Delete methods are overridden to throw NotSupportedException (BR-010).
/// Ledger entries can only be created and read — never modified or deleted.
/// </summary>
public interface IInventoryLedgerRepository : IRepository<InventoryLedgerEntry, Guid>
{
    /// <summary>⚠️ NotSupportedException — ledger entries cannot be updated.</summary>
    Task<InventoryLedgerEntry> UpdateAsync(InventoryLedgerEntry entity, bool autoSave = false, CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException("Inventory ledger entries cannot be updated (BR-010).");
    }

    /// <summary>⚠️ NotSupportedException — ledger entries cannot be deleted.</summary>
    Task DeleteAsync(InventoryLedgerEntry entity, bool autoSave = false, CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException("Inventory ledger entries cannot be deleted (BR-010).");
    }

    /// <summary>⚠️ NotSupportedException — ledger entries cannot be deleted by ID.</summary>
    Task DeleteAsync(Guid id, bool autoSave = false, CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException("Inventory ledger entries cannot be deleted (BR-010).");
    }

    /// <summary>Get all ledger entries for a specific balance.</summary>
    Task<List<InventoryLedgerEntry>> GetByBalanceIdAsync(Guid inventoryBalanceId, int maxResultCount = 100, int skipCount = 0);

    /// <summary>Get ledger entries by source order.</summary>
    Task<List<InventoryLedgerEntry>> GetBySourceOrderAsync(string sourceOrderType, Guid sourceOrderId);

    /// <summary>Get ledger entries within a time range.</summary>
    Task<List<InventoryLedgerEntry>> GetByTimeRangeAsync(DateTime startTime, DateTime endTime, int maxResultCount = 100, int skipCount = 0);

    /// <summary>Get ledger entries for a material within a time range.</summary>
    Task<List<InventoryLedgerEntry>> GetByMaterialAsync(Guid materialId, DateTime? startTime = null, DateTime? endTime = null, int maxResultCount = 100, int skipCount = 0);
}
