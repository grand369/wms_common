namespace Wms.Notification.Domain.Enums;

public sealed class NotificationChannel : SmartEnum<NotificationChannel, int>
{
    public static readonly NotificationChannel Internal = new("Internal", 0, "站内消息");
    public static readonly NotificationChannel Email = new("Email", 1, "邮件");
    public static readonly NotificationChannel Sms = new("Sms", 2, "短信");
    public static readonly NotificationChannel WeChatWork = new("WeChatWork", 3, "企业微信");
    public static readonly NotificationChannel DingTalk = new("DingTalk", 4, "钉钉");

    public string Description { get; }

    private NotificationChannel(string name, int value, string description) : base(name, value)
    {
        Description = description;
    }
}
