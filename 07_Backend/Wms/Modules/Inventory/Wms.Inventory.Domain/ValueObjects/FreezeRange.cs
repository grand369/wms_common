namespace Wms.Inventory.Domain.ValueObjects;

/// <summary>
/// Freeze Range Value Object — defines the scope of an inventory freeze operation.
/// Different combinations based on FreezeScope: ByBatch, ByMaterial, ByLocation, ByWarehouse.
/// (AGG-09, Phase 3 DDD Design)
/// </summary>
public class FreezeRange
{
    public Guid? MaterialId { get; set; }
    public string? MaterialCode { get; set; }
    public Guid? WarehouseId { get; set; }
    public string? WarehouseCode { get; set; }
    public Guid? LocationId { get; set; }
    public string? LocationCode { get; set; }
    public string? BatchNumber { get; set; }
}
