namespace Wms.Inventory.Application.Contracts.Dtos;

/// <summary>
/// Inventory Adjustment Query DTO — parameters for filtering adjustment queries.
/// </summary>
public class InventoryAdjustmentQueryDto
{
    public Guid? WarehouseId { get; set; }
    public int? ApprovalStatusValue { get; set; }
    public string? Keyword { get; set; }
    public int SkipCount { get; set; } = 0;
    public int MaxResultCount { get; set; } = 20;
}
