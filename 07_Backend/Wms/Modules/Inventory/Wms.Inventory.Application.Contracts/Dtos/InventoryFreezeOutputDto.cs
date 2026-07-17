namespace Wms.Inventory.Application.Contracts.Dtos;

/// <summary>
/// Inventory Freeze Output DTO — flattened view of freeze order.
/// </summary>
public class InventoryFreezeOutputDto
{
    public Guid Id { get; set; }
    public string FreezeOrderNo { get; set; } = string.Empty;
    public int FreezeScopeValue { get; set; }
    public string FreezeScopeName { get; set; } = string.Empty;
    public string FreezeReason { get; set; } = string.Empty;
    public int FreezeStatusValue { get; set; }
    public string FreezeStatusName { get; set; } = string.Empty;
    public Guid WarehouseId { get; set; }
    public string WarehouseCode { get; set; } = string.Empty;
    public bool IsApproved { get; set; }
    public DateTime FreezeStartTime { get; set; }
    public DateTime? FreezeEndTime { get; set; }
    public string? Remark { get; set; }
    public DateTime CreationTime { get; set; }
}
