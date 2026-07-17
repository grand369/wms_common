namespace Wms.Material.Domain.Enums;

/// <summary>
/// Issue Strategy Type Smart Enum — defines the material issue/picking strategy.
/// (VO-10, Phase 3 DDD Design)
/// </summary>
public sealed class IssueStrategyType : SmartEnum<IssueStrategyType, int>
{
    public static readonly IssueStrategyType FIFO = new IssueStrategyType("FIFO", 0, "先进先出");
    public static readonly IssueStrategyType FEFO = new IssueStrategyType("FEFO", 1, "先过期先出");
    public static readonly IssueStrategyType FMFO = new IssueStrategyType("FMFO", 2, "先生产先出");
    public static readonly IssueStrategyType Manual = new IssueStrategyType("Manual", 3, "手动指定");

    public string Description { get; }

    private IssueStrategyType(string name, int value, string description) : base(name, value)
    {
        Description = description;
    }
}
