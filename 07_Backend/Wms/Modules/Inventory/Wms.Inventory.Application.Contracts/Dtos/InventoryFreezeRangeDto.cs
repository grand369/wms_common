namespace Wms.Inventory.Application.Contracts.Dtos;

/// <summary>
/// Inventory Freeze Range DTO — defines the scope of a freeze operation.
/// </summary>
public class InventoryFreezeRangeDto
{
    public Guid? MaterialId { get; set; }
    public string? MaterialCode { get; set; }
    public Guid? WarehouseId { get; set; }
    public string? WarehouseCode { get; set; }
    public Guid? LocationId { get; set; }
    public string? LocationCode { get; set; }
    public string? BatchNumber { get; set; }
}
