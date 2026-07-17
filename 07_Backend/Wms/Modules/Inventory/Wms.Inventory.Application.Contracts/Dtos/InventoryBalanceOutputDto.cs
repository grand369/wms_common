using Wms.Shared.Domain.Enums;

namespace Wms.Inventory.Application.Contracts.Dtos;

/// <summary>
/// Inventory Balance Output DTO (DTO-IV-001) — flattened view of all balance fields.
/// Core DTO for inventory queries.
/// </summary>
public class InventoryBalanceOutputDto
{
    public Guid Id { get; set; }
    public Guid MaterialId { get; set; }
    public string MaterialCode { get; set; } = string.Empty;
    public Guid WarehouseId { get; set; }
    public string WarehouseCode { get; set; } = string.Empty;
    public Guid LocationId { get; set; }
    public string LocationCode { get; set; } = string.Empty;
    public string? BatchNumber { get; set; }
    public int InventoryStatusValue { get; set; }
    public string InventoryStatusName { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public decimal ReservedQuantity { get; set; }
    public decimal FrozenQuantity { get; set; }
    public decimal InTransitQuantity { get; set; }
    public decimal AvailableQuantity { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public DateTime? ProductionDate { get; set; }
    public Guid? SupplierId { get; set; }
    public string? SupplierName { get; set; }
    public decimal? UnitCost { get; set; }
    public DateTime LastOperationTime { get; set; }
    public int ConcurrencyVersion { get; set; }
    public DateTime CreationTime { get; set; }
}
