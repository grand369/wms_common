namespace Wms.Notification.Domain.Enums;

public sealed class ReadStatus : SmartEnum<ReadStatus, int>
{
    public static readonly ReadStatus Unread = new("Unread", 0, "未读");
    public static readonly ReadStatus Read = new("Read", 1, "已读");

    public string Description { get; }

    private ReadStatus(string name, int value, string description) : base(name, value)
    {
        Description = description;
    }
}
