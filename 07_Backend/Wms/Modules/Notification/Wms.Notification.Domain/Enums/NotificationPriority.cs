namespace Wms.Notification.Domain.Enums;

public sealed class NotificationPriority : SmartEnum<NotificationPriority, int>
{
    public static readonly NotificationPriority Low = new("Low", 0, "低");
    public static readonly NotificationPriority Normal = new("Normal", 1, "普通");
    public static readonly NotificationPriority High = new("High", 2, "高");
    public static readonly NotificationPriority Emergency = new("Emergency", 3, "紧急");

    public string Description { get; }

    private NotificationPriority(string name, int value, string description) : base(name, value)
    {
        Description = description;
    }
}
