using Wms.Shared.Domain.Enums;

namespace Wms.Inventory.Domain.Enums;

/// <summary>
/// Freeze Status SmartEnum — defines the lifecycle states of a freeze order.
/// (AGG-09, Phase 3 DDD Design)
/// </summary>
public sealed class FreezeStatus : SmartEnum<FreezeStatus, int>
{
    public static readonly FreezeStatus Active =
        new FreezeStatus("Active", 0, "冻结中");
    public static readonly FreezeStatus Released =
        new FreezeStatus("Released", 1, "已释放");
    public static readonly FreezeStatus Cancelled =
        new FreezeStatus("Cancelled", 2, "已取消");

    public string Description { get; }

    private FreezeStatus(string name, int value, string description) : base(name, value)
    {
        Description = description;
    }
}
