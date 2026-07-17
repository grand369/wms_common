using Wms.Shared.Domain.Enums;

namespace Wms.Inventory.Domain.Enums;

/// <summary>
/// Alert Type SmartEnum — defines the types of inventory alerts.
/// (AGG-09 extension, Phase 3 DDD Design)
/// </summary>
public sealed class AlertType : SmartEnum<AlertType, int>
{
    public static readonly AlertType SafetyStock =
        new AlertType("SafetyStock", 0, "安全库存预警");
    public static readonly AlertType Expiry =
        new AlertType("Expiry", 1, "临期预警");
    public static readonly AlertType ZeroInventory =
        new AlertType("ZeroInventory", 2, "零库存预警");
    public static readonly AlertType Overstock =
        new AlertType("Overstock", 3, "超储预警");
    public static readonly AlertType Age =
        new AlertType("Age", 4, "库龄预警");

    public string Description { get; }

    private AlertType(string name, int value, string description) : base(name, value)
    {
        Description = description;
    }
}
