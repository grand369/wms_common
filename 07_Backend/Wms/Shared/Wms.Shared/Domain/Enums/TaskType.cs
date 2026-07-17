namespace Wms.Shared.Domain.Enums;

/// <summary>
/// Task Type Smart Enum — defines types of warehouse operations.
/// Shared across TaskCenter, Inbound, Outbound modules.
/// </summary>
public sealed class TaskType : SmartEnum<TaskType, int>
{
    public static readonly TaskType Putaway = new TaskType("Putaway", 1, "上架");
    public static readonly TaskType Picking = new TaskType("Picking", 2, "拣货");
    public static readonly TaskType Transfer = new TaskType("Transfer", 3, "移库");
    public static readonly TaskType CycleCount = new TaskType("CycleCount", 4, "盘点");
    public static readonly TaskType QualityInspection = new TaskType("QualityInspection", 5, "质检");
    public static readonly TaskType Replenishment = new TaskType("Replenishment", 6, "补料");
    public static readonly TaskType Packing = new TaskType("Packing", 7, "打包");
    public static readonly TaskType Shipping = new TaskType("Shipping", 8, "发货");

    public string Description { get; }

    private TaskType(string name, int value, string description) : base(name, value)
    {
        Description = description;
    }
}
