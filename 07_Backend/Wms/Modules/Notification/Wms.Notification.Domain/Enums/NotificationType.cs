namespace Wms.Notification.Domain.Enums;

public sealed class NotificationType : SmartEnum<NotificationType, int>
{
    public static readonly NotificationType Alert = new("Alert", 0, "告警");
    public static readonly NotificationType Approval = new("Approval", 1, "审批");
    public static readonly NotificationType TaskAssignment = new("TaskAssignment", 2, "任务分配");
    public static readonly NotificationType System = new("System", 3, "系统通知");

    public string Description { get; }

    private NotificationType(string name, int value, string description) : base(name, value)
    {
        Description = description;
    }
}
