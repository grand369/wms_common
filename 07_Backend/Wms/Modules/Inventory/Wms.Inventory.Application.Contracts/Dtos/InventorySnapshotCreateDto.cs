namespace Wms.Inventory.Application.Contracts.Dtos;

/// <summary>
/// Create DTO for inventory snapshot.
/// </summary>
public class InventorySnapshotCreateDto
{
    public Guid WarehouseId { get; set; }
    public string? Remark { get; set; }
}
