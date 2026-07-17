namespace Wms.Inventory.Application.Contracts.Dtos;

/// <summary>
/// Inventory Balance Initialize DTO — request body for initializing a new inventory balance.
/// </summary>
public class InventoryBalanceInitializeDto
{
    public Guid MaterialId { get; set; }
    public string MaterialCode { get; set; } = string.Empty;
    public Guid WarehouseId { get; set; }
    public string WarehouseCode { get; set; } = string.Empty;
    public Guid LocationId { get; set; }
    public string LocationCode { get; set; } = string.Empty;
    public string? BatchNumber { get; set; }
    public decimal Quantity { get; set; }
    public decimal UnitCost { get; set; }
    public Guid? SupplierId { get; set; }
    public string? SupplierName { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public DateTime? ProductionDate { get; set; }
    public bool AllowNegativeInventory { get; set; } = false;
    public string SourceOrderType { get; set; } = "InventoryInitialization";
    public Guid SourceOrderId { get; set; } = Guid.Empty;
}
