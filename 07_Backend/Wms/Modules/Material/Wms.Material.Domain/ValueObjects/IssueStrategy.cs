namespace Wms.Material.Domain.ValueObjects;

/// <summary>
/// Issue Strategy Value Object (VO-10) — represents the material issue/picking strategy configuration.
/// Stored as JSON column in Material table (nvarchar(max)).
/// (ENT-04, Phase 3 DDD Design)
/// </summary>
public record IssueStrategy
{
    public int IssueStrategyType { get; init; }
    public int StrategyScope { get; init; }

    public IssueStrategy() { }

    public IssueStrategy(int issueStrategyType = 0, int strategyScope = 0)
    {
        IssueStrategyType = issueStrategyType;
        StrategyScope = strategyScope;
    }
}
