namespace Wms.Inventory.Application.Contracts.Dtos;

/// <summary>
/// Inventory Adjustment Create DTO — request body for creating a new adjustment.
/// Contains the adjustment header and line items.
/// </summary>
public class InventoryAdjustmentCreateDto
{
    public string AdjustmentNo { get; set; } = string.Empty;
    public int AdjustmentTypeValue { get; set; }
    public string AdjustmentReason { get; set; } = string.Empty;
    public Guid WarehouseId { get; set; }
    public string WarehouseCode { get; set; } = string.Empty;
    public string? Remark { get; set; }
    public List<InventoryAdjustmentLineDto> Lines { get; set; } = new();
}
