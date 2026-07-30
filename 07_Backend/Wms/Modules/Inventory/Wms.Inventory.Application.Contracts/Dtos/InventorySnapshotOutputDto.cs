namespace Wms.Inventory.Application.Contracts.Dtos;

/// <summary>
/// Output DTO for inventory snapshot.
/// </summary>
public class InventorySnapshotOutputDto
{
    public Guid Id { get; set; }
    public string SnapshotNo { get; set; } = string.Empty;
    public Guid WarehouseId { get; set; }
    public string WarehouseCode { get; set; } = string.Empty;
    public string? WarehouseName { get; set; }
    public DateTime SnapshotTime { get; set; }
    public decimal TotalQty { get; set; }
    public decimal TotalFrozenQty { get; set; }
    public decimal TotalAvailableQty { get; set; }
    public int Status { get; set; }
    public string? Remark { get; set; }
    public DateTime CreationTime { get; set; }
}
