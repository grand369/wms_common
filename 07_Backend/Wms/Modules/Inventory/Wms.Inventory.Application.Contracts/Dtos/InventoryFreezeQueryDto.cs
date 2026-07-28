namespace Wms.Inventory.Application.Contracts.Dtos;

/// <summary>
/// Inventory Freeze Query DTO — parameters for filtering freeze order queries.
/// </summary>
public class InventoryFreezeQueryDto
{
    public Guid? WarehouseId { get; set; }
    public int? FreezeStatusValue { get; set; }
    public string? FreezeOrderNo { get; set; }
    public string? MaterialCode { get; set; }
    public string? Keyword { get; set; }
    public int SkipCount { get; set; } = 0;
    public int MaxResultCount { get; set; } = 20;
}
