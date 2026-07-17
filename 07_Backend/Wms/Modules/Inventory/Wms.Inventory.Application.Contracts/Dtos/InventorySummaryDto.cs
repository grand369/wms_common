namespace Wms.Inventory.Application.Contracts.Dtos;

/// <summary>
/// Inventory Summary DTO — aggregated statistics about inventory across the platform.
/// </summary>
public class InventorySummaryDto
{
    public int TotalBalanceCount { get; set; }
    public decimal TotalQuantity { get; set; }
    public decimal TotalAvailableQuantity { get; set; }
    public decimal TotalReservedQuantity { get; set; }
    public decimal TotalFrozenQuantity { get; set; }
    public decimal TotalInTransitQuantity { get; set; }
    public int MaterialCount { get; set; }
    public int ZeroInventoryCount { get; set; }
    public int NearExpiryCount { get; set; }
    public int BelowSafetyStockCount { get; set; }
}
