namespace Wms.Material.Domain.ValueObjects;

/// <summary>
/// Inventory Attribute Value Object (VO-13) — represents the inventory-related attributes of a material.
/// Stored as JSON column in Material table (nvarchar(max)).
/// (ENT-04, Phase 3 DDD Design)
/// </summary>
public record InventoryAttribute
{
    public decimal SafetyStockQuantity { get; init; }
    public decimal MinOrderQuantity { get; init; }
    public int ABCClassification { get; init; }
    public bool AllowNegativeInventory { get; init; }

    public InventoryAttribute() { }

    public InventoryAttribute(
        decimal safetyStockQuantity = 0,
        decimal minOrderQuantity = 0,
        int abcClassification = 2,
        bool allowNegativeInventory = false)
    {
        SafetyStockQuantity = safetyStockQuantity;
        MinOrderQuantity = minOrderQuantity;
        ABCClassification = abcClassification;
        AllowNegativeInventory = allowNegativeInventory;
    }
}
