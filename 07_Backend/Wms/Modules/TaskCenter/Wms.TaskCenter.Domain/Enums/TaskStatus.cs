namespace Wms.TaskCenter.Domain.Enums;

/// <summary>
/// Task Status Smart Enum — SM-03 state machine for WarehouseTask lifecycle.
/// Created → Assigned → InProgress → Completed
///   + Suspended (from InProgress) → InProgress (resume) / Cancelled (close)
///   + Cancelled (from Created/Assigned)
/// </summary>
public sealed class TaskStatus : SmartEnum<TaskStatus, int>
{
    public static readonly TaskStatus Created = new TaskStatus("Created", 0, "已创建");
    public static readonly TaskStatus Assigned = new TaskStatus("Assigned", 1, "已分配");
    public static readonly TaskStatus InProgress = new TaskStatus("InProgress", 2, "进行中");
    public static readonly TaskStatus Suspended = new TaskStatus("Suspended", 3, "已挂起");
    public static readonly TaskStatus Completed = new TaskStatus("Completed", 4, "已完成");
    public static readonly TaskStatus Cancelled = new TaskStatus("Cancelled", 5, "已取消");

    public string Description { get; }

    private TaskStatus(string name, int value, string description) : base(name, value)
    {
        Description = description;
    }
}
