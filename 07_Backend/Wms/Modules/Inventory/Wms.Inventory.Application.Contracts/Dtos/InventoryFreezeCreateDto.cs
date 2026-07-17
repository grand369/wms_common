namespace Wms.Inventory.Application.Contracts.Dtos;

/// <summary>
/// Inventory Freeze Create DTO — request body for creating a freeze order.
/// Contains FreezeScope and FreezeRanges.
/// </summary>
public class InventoryFreezeCreateDto
{
    public string FreezeOrderNo { get; set; } = string.Empty;
    public int FreezeScopeValue { get; set; }
    public string FreezeReason { get; set; } = string.Empty;
    public Guid WarehouseId { get; set; }
    public string WarehouseCode { get; set; } = string.Empty;
    public DateTime FreezeStartTime { get; set; }
    public DateTime? FreezeEndTime { get; set; }
    public string? Remark { get; set; }
    public List<InventoryFreezeRangeDto> FreezeRanges { get; set; } = new();
}
