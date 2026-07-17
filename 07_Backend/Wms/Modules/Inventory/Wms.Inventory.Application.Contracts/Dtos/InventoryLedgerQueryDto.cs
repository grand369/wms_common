namespace Wms.Inventory.Application.Contracts.Dtos;

/// <summary>
/// Inventory Ledger Query DTO — parameters for filtering ledger entry queries.
/// </summary>
public class InventoryLedgerQueryDto
{
    public Guid? BalanceId { get; set; }
    public string? SourceOrderType { get; set; }
    public Guid? SourceOrderId { get; set; }
    public Guid? MaterialId { get; set; }
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public int SkipCount { get; set; } = 0;
    public int MaxResultCount { get; set; } = 20;
}
