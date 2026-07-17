namespace Wms.Material.Domain.Enums;

/// <summary>
/// Strategy Scope Smart Enum — defines the scope of the issue strategy.
/// (VO-10, Phase 3 DDD Design)
/// </summary>
public sealed class StrategyScope : SmartEnum<StrategyScope, int>
{
    public static readonly StrategyScope ByMaterial = new StrategyScope("ByMaterial", 0, "按物料");
    public static readonly StrategyScope ByWarehouse = new StrategyScope("ByWarehouse", 1, "按仓库");
    public static readonly StrategyScope ByArea = new StrategyScope("ByArea", 2, "按库区");

    public string Description { get; }

    private StrategyScope(string name, int value, string description) : base(name, value)
    {
        Description = description;
    }
}
