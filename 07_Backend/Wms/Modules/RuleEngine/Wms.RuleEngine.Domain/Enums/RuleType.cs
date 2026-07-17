using Wms.Shared.Domain.Enums;

namespace Wms.RuleEngine.Domain.Enums;

/// <summary>
/// RuleType SmartEnum — defines the types of business rules.
/// QualityInspection=0, PutawayStrategy=1, IssueStrategy=2, AlertThreshold=3
/// </summary>
public sealed class RuleType : SmartEnum<RuleType, int>
{
    public static readonly RuleType QualityInspection = new(0, "QualityInspection", "质量检验");
    public static readonly RuleType PutawayStrategy = new(1, "PutawayStrategy", "上架策略");
    public static readonly RuleType IssueStrategy = new(2, "IssueStrategy", "出库策略");
    public static readonly RuleType AlertThreshold = new(3, "AlertThreshold", "预警阈值");

    public string Description { get; }

    private RuleType(int value, string name, string description) : base(name, value)
    {
        Description = description;
    }
}
