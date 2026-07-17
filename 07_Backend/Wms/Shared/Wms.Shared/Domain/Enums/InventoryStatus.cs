namespace Wms.Shared.Domain.Enums;

/// <summary>
/// Inventory Status Smart Enum (VO-08) — tracks the status of inventory items.
/// Shared across Inventory, Inbound, Outbound, CycleCount modules.
/// </summary>
public sealed class InventoryStatus : SmartEnum<InventoryStatus, int>
{
    public static readonly InventoryStatus Available = new InventoryStatus("Available", 0, "可用");
    public static readonly InventoryStatus Reserved = new InventoryStatus("Reserved", 1, "预留");
    public static readonly InventoryStatus Frozen = new InventoryStatus("Frozen", 2, "冻结");
    public static readonly InventoryStatus InTransit = new InventoryStatus("InTransit", 3, "在途");
    public static readonly InventoryStatus QualityHold = new InventoryStatus("QualityHold", 4, "质检待判");
    public static readonly InventoryStatus Scrapped = new InventoryStatus("Scrapped", 5, "报废");

    public string Description { get; }

    private InventoryStatus(string name, int value, string description) : base(name, value)
    {
        Description = description;
    }
}
