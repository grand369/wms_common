namespace Wms.Material.Domain.ValueObjects;

/// <summary>
/// Storage Attribute Value Object (VO-11) — represents the storage-related attributes of a material.
/// Stored as JSON column in Material table (nvarchar(max)).
/// (ENT-04, Phase 3 DDD Design)
/// </summary>
public record StorageAttribute
{
    public int StorageConditionType { get; init; }
    public int MaxStackingLayers { get; init; }
    public string PackageSpec { get; init; } = string.Empty;
    public decimal WeightPerUnit { get; init; }

    public StorageAttribute() { }

    public StorageAttribute(int storageConditionType, int maxStackingLayers = 1, string packageSpec = "", decimal weightPerUnit = 0)
    {
        StorageConditionType = storageConditionType;
        MaxStackingLayers = maxStackingLayers;
        PackageSpec = packageSpec ?? string.Empty;
        WeightPerUnit = weightPerUnit;
    }
}
