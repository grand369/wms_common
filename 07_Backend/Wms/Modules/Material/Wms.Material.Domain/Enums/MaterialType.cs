namespace Wms.Material.Domain.Enums;

/// <summary>
/// Material Type Smart Enum — defines the type/category of material.
/// 8 types covering manufacturing material scenarios.
/// (ENT-04, Phase 3 DDD Design)
/// </summary>
public sealed class MaterialType : SmartEnum<MaterialType, int>
{
    public static readonly MaterialType RawMaterial = new MaterialType("RawMaterial", 0, "原材料");
    public static readonly MaterialType SemiFinished = new MaterialType("SemiFinished", 1, "半成品");
    public static readonly MaterialType Finished = new MaterialType("Finished", 2, "成品");
    public static readonly MaterialType Auxiliary = new MaterialType("Auxiliary", 3, "辅料");
    public static readonly MaterialType SparePart = new MaterialType("SparePart", 4, "备件");
    public static readonly MaterialType Consumable = new MaterialType("Consumable", 5, "消耗品");
    public static readonly MaterialType Packaging = new MaterialType("Packaging", 6, "包装材料");
    public static readonly MaterialType Hazardous = new MaterialType("Hazardous", 7, "危险品");

    public string Description { get; }

    private MaterialType(string name, int value, string description) : base(name, value)
    {
        Description = description;
    }
}
