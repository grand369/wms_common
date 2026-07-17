namespace Wms.Shared.Domain.Enums;

/// <summary>
/// Task Priority Smart Enum — defines priority levels for warehouse tasks.
/// Shared across TaskCenter, Inbound, Outbound, Transfer modules.
/// </summary>
public sealed class TaskPriority : SmartEnum<TaskPriority, int>
{
    public static readonly TaskPriority Low = new TaskPriority("Low", 1, "低");
    public static readonly TaskPriority Medium = new TaskPriority("Medium", 2, "中");
    public static readonly TaskPriority High = new TaskPriority("High", 3, "高");
    public static readonly TaskPriority Emergency = new TaskPriority("Emergency", 4, "紧急");

    public string Description { get; }

    private TaskPriority(string name, int value, string description) : base(name, value)
    {
        Description = description;
    }
}
