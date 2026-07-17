namespace Wms.Inventory.Application.Contracts.Dtos;

/// <summary>
/// Inventory Adjustment Line DTO — a single line item in an adjustment.
/// </summary>
public class InventoryAdjustmentLineDto
{
    public int LineNo { get; set; }
    public Guid MaterialId { get; set; }
    public string MaterialCode { get; set; } = string.Empty;
    public string MaterialName { get; set; } = string.Empty;
    public decimal AdjustmentQuantity { get; set; }
    public Guid LocationId { get; set; }
    public string LocationCode { get; set; } = string.Empty;
    public string? BatchNumber { get; set; }
    public int InventoryStatusBeforeValue { get; set; }
    public int InventoryStatusAfterValue { get; set; }
    public string? Reason { get; set; }
}
