namespace Wms.Inventory.Application.Contracts.Dtos;

/// <summary>
/// Inventory Alert Output DTO — flattened view of alert records.
/// </summary>
public class InventoryAlertOutputDto
{
    public Guid Id { get; set; }
    public int AlertTypeValue { get; set; }
    public string AlertTypeName { get; set; } = string.Empty;
    public Guid MaterialId { get; set; }
    public string MaterialCode { get; set; } = string.Empty;
    public Guid WarehouseId { get; set; }
    public string WarehouseCode { get; set; } = string.Empty;
    public decimal CurrentQuantity { get; set; }
    public decimal ThresholdQuantity { get; set; }
    public bool IsResolved { get; set; }
    public DateTime AlertTime { get; set; }
    public DateTime? ResolveTime { get; set; }
    public DateTime CreationTime { get; set; }
}
