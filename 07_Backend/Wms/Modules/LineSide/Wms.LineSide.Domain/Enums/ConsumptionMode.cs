namespace Wms.LineSide.Domain.Enums;

/// <summary>
/// Consumption Mode Smart Enum — how line-side consumption is tracked.
/// Scan = operator scans each item; Backflush = auto-consumed by production order completion
/// </summary>
public sealed class ConsumptionMode : Wms.Shared.Domain.Enums.SmartEnum<ConsumptionMode, int>
{
    public static readonly ConsumptionMode Scan = new ConsumptionMode("Scan", 1, "扫码消耗");
    public static readonly ConsumptionMode Backflush = new ConsumptionMode("Backflush", 2, "倒推消耗");

    public string Description { get; }
    private ConsumptionMode(string name, int value, string description) : base(name, value) { Description = description; }
}
