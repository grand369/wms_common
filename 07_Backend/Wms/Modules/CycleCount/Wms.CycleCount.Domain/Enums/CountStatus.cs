namespace Wms.CycleCount.Domain.Enums;

/// <summary>
/// Count Status Smart Enum — tracks cycle count plan lifecycle
/// Planned → InProgress → Completed → Closed
/// </summary>
public sealed class CountStatus : Wms.Shared.Domain.Enums.SmartEnum<CountStatus, int>
{
    public static readonly CountStatus Planned = new CountStatus("Planned", 0, "已计划");
    public static readonly CountStatus InProgress = new CountStatus("InProgress", 1, "盘点中");
    public static readonly CountStatus Completed = new CountStatus("Completed", 2, "已完成");
    public static readonly CountStatus Closed = new CountStatus("Closed", 3, "已关闭");

    public string Description { get; }
    private CountStatus(string name, int value, string description) : base(name, value) { Description = description; }
}
