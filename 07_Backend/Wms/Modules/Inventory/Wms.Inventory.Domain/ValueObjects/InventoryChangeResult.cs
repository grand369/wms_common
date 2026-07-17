using Wms.Inventory.Domain.Aggregates;

namespace Wms.Inventory.Domain.ValueObjects;

/// <summary>
/// Inventory Change Result — the return value of ApplyQuantityChange().
/// Contains the updated balance info and the ledger entry that was generated.
/// (AGG-06, Phase 3 DDD Design)
/// </summary>
public class InventoryChangeResult
{
    public Guid BalanceId { get; set; }
    public InventoryLedgerEntry? LedgerEntry { get; set; }
    public decimal ChangeQuantity { get; set; }
    public decimal BeforeQuantity { get; set; }
    public decimal AfterQuantity { get; set; }
}
