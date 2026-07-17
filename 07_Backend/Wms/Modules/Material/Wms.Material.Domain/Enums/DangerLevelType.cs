namespace Wms.Material.Domain.Enums;

/// <summary>
/// Danger Level Type Smart Enum — defines the danger/hazard level of a material.
/// (VO-14, Phase 3 DDD Design)
/// </summary>
public sealed class DangerLevelType : SmartEnum<DangerLevelType, int>
{
    public static readonly DangerLevelType None = new DangerLevelType("None", 0, "无危险");
    public static readonly DangerLevelType Low = new DangerLevelType("Low", 1, "低危险");
    public static readonly DangerLevelType Medium = new DangerLevelType("Medium", 2, "中危险");
    public static readonly DangerLevelType High = new DangerLevelType("High", 3, "高危险");
    public static readonly DangerLevelType Extreme = new DangerLevelType("Extreme", 4, "极高危险");

    public string Description { get; }

    private DangerLevelType(string name, int value, string description) : base(name, value)
    {
        Description = description;
    }
}
