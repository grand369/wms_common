using Wms.Shared.Domain.Enums;

namespace Wms.Inventory.Domain.Enums;

/// <summary>
/// Adjustment Type SmartEnum — defines the types of inventory adjustments.
/// (AGG-08, Phase 3 DDD Design)
/// </summary>
public sealed class AdjustmentType : SmartEnum<AdjustmentType, int>
{
    public static readonly AdjustmentType Gain =
        new AdjustmentType("Gain", 0, "盘盈");
    public static readonly AdjustmentType Loss =
        new AdjustmentType("Loss", 1, "盘亏");
    public static readonly AdjustmentType Scrap =
        new AdjustmentType("Scrap", 2, "报废");
    public static readonly AdjustmentType Revaluation =
        new AdjustmentType("Revaluation", 3, "重估");

    public string Description { get; }

    private AdjustmentType(string name, int value, string description) : base(name, value)
    {
        Description = description;
    }
}
