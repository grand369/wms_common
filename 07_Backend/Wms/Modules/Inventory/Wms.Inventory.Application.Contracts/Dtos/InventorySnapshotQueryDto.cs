namespace Wms.Inventory.Application.Contracts.Dtos;

/// <summary>
/// Query DTO for inventory snapshot list.
/// </summary>
public class InventorySnapshotQueryDto
{
    public Guid? WarehouseId { get; set; }
    public int? Status { get; set; }
    public string? Keyword { get; set; }
    public int SkipCount { get; set; } = 0;
    public int MaxResultCount { get; set; } = 20;
}
