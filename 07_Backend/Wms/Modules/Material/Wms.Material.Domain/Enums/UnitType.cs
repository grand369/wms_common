namespace Wms.Material.Domain.Enums;

/// <summary>
/// Unit Type Smart Enum — defines the category of a unit of measure.
/// Used in UnitOfMeasure entity.
/// (ENT-04 reference, Phase 3 DDD Design)
/// </summary>
public sealed class UnitType : SmartEnum<UnitType, int>
{
    public static readonly UnitType Length = new UnitType("Length", 0, "长度");
    public static readonly UnitType Weight = new UnitType("Weight", 1, "重量");
    public static readonly UnitType Volume = new UnitType("Volume", 2, "体积");
    public static readonly UnitType Time = new UnitType("Time", 3, "时间");
    public static readonly UnitType Count = new UnitType("Count", 4, "计数");
    public static readonly UnitType Area = new UnitType("Area", 5, "面积");
    public static readonly UnitType Temperature = new UnitType("Temperature", 6, "温度");

    public string Description { get; }

    private UnitType(string name, int value, string description) : base(name, value)
    {
        Description = description;
    }
}
