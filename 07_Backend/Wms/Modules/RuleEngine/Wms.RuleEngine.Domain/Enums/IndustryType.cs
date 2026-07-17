using Wms.Shared.Domain.Enums;

namespace Wms.RuleEngine.Domain.Enums;

/// <summary>
/// IndustryType SmartEnum — defines industry types for configuration packages.
/// Automotive=0, Electronics=1, Food=2, Pharmaceutical=3, General=4
/// </summary>
public sealed class IndustryType : SmartEnum<IndustryType, int>
{
    public static readonly IndustryType Automotive = new(0, "Automotive", "汽车");
    public static readonly IndustryType Electronics = new(1, "Electronics", "电子");
    public static readonly IndustryType Food = new(2, "Food", "食品");
    public static readonly IndustryType Pharmaceutical = new(3, "Pharmaceutical", "制药");
    public static readonly IndustryType General = new(4, "General", "通用");

    public string Description { get; }

    private IndustryType(int value, string name, string description) : base(name, value)
    {
        Description = description;
    }
}
