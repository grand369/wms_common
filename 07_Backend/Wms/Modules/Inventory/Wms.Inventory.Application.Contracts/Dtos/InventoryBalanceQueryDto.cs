namespace Wms.Inventory.Application.Contracts.Dtos;

/// <summary>
/// Inventory Balance Query DTO — parameters for filtering and paginating balance queries.
/// </summary>
public class InventoryBalanceQueryDto
{
    public Guid? MaterialId { get; set; }
    public Guid? WarehouseId { get; set; }
    public Guid? LocationId { get; set; }
    public string? BatchNumber { get; set; }
    public int? InventoryStatusValue { get; set; }
    public string? Keyword { get; set; }
    public int SkipCount { get; set; } = 0;
    public int MaxResultCount { get; set; } = 20;
}
