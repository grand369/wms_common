namespace Wms.Warehouse.Domain.Enums;

/// <summary>
/// Location Type Smart Enum — defines the physical type of a storage location.
/// (ENT-03, Phase 3 DDD Design)
/// </summary>
public sealed class LocationType : SmartEnum<LocationType, int>
{
    public static readonly LocationType Standard = new LocationType("Standard", 0, "标准库位");
    public static readonly LocationType Shelf = new LocationType("Shelf", 1, "货架库位");
    public static readonly LocationType Grid = new LocationType("Grid", 2, "货格库位");
    public static readonly LocationType Pallet = new LocationType("Pallet", 3, "托盘库位");
    public static readonly LocationType Staging = new LocationType("Staging", 4, "暂存库位");

    public string Description { get; }

    private LocationType(string name, int value, string description) : base(name, value)
    {
        Description = description;
    }
}
