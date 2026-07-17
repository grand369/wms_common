namespace Wms.Inventory.Application.Contracts.Dtos;

/// <summary>
/// Inventory Balance Available Query DTO — parameters for available quantity aggregation queries.
/// </summary>
public class InventoryBalanceAvailableQueryDto
{
    public Guid? MaterialId { get; set; }
    public Guid? WarehouseId { get; set; }
    public Guid? LocationId { get; set; }
}
