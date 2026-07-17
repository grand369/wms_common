using Wms.Shared.Domain.Enums;

namespace Wms.Inventory.Domain.Enums;

/// <summary>
/// Freeze Scope SmartEnum — defines the granularity of inventory freeze operations.
/// (AGG-09, Phase 3 DDD Design)
/// </summary>
public sealed class FreezeScope : SmartEnum<FreezeScope, int>
{
    public static readonly FreezeScope ByBatch =
        new FreezeScope("ByBatch", 0, "按批次");
    public static readonly FreezeScope ByMaterial =
        new FreezeScope("ByMaterial", 1, "按物料");
    public static readonly FreezeScope ByLocation =
        new FreezeScope("ByLocation", 2, "按库位");
    public static readonly FreezeScope ByWarehouse =
        new FreezeScope("ByWarehouse", 3, "按仓库");

    public string Description { get; }

    private FreezeScope(string name, int value, string description) : base(name, value)
    {
        Description = description;
    }
}
