namespace Wms.Material.Domain.Enums;

/// <summary>
/// ABC Classification Type Smart Enum — defines the ABC classification category.
/// (VO-13, Phase 3 DDD Design)
/// </summary>
public sealed class ABCClassificationType : SmartEnum<ABCClassificationType, int>
{
    public static readonly ABCClassificationType A = new ABCClassificationType("A", 0, "A类（高价值/关键物料）");
    public static readonly ABCClassificationType B = new ABCClassificationType("B", 1, "B类（中等价值/一般物料）");
    public static readonly ABCClassificationType C = new ABCClassificationType("C", 2, "C类（低价值/辅助物料）");

    public string Description { get; }

    private ABCClassificationType(string name, int value, string description) : base(name, value)
    {
        Description = description;
    }
}
