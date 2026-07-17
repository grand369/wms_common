namespace Wms.Inventory.Application.Contracts.Dtos;

/// <summary>
/// Inventory Adjustment Output DTO — includes header and line items.
/// </summary>
public class InventoryAdjustmentOutputDto
{
    public Guid Id { get; set; }
    public string AdjustmentNo { get; set; } = string.Empty;
    public int AdjustmentTypeValue { get; set; }
    public string AdjustmentTypeName { get; set; } = string.Empty;
    public string AdjustmentReason { get; set; } = string.Empty;
    public int ApprovalStatusValue { get; set; }
    public string ApprovalStatusName { get; set; } = string.Empty;
    public Guid WarehouseId { get; set; }
    public string WarehouseCode { get; set; } = string.Empty;
    public bool IsCompleted { get; set; }
    public DateTime? CompletionTime { get; set; }
    public string? Remark { get; set; }
    public List<InventoryAdjustmentLineDto> Lines { get; set; } = new();
    public DateTime CreationTime { get; set; }
}
