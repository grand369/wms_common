namespace Wms.Notification.Domain.Enums;

public sealed class SendStatus : SmartEnum<SendStatus, int>
{
    public static readonly SendStatus Pending = new("Pending", 0, "待发送");
    public static readonly SendStatus Sent = new("Sent", 1, "已发送");
    public static readonly SendStatus Failed = new("Failed", 2, "发送失败");
    public static readonly SendStatus Retrying = new("Retrying", 3, "重试中");

    public string Description { get; }

    private SendStatus(string name, int value, string description) : base(name, value)
    {
        Description = description;
    }
}
