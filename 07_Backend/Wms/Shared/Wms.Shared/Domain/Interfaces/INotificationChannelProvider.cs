namespace Wms.Shared.Domain.Interfaces;

/// <summary>
/// INotificationChannelProvider — extension point for adding new notification channels.
/// Each channel provider (Email, Sms, WeChatWork, DingTalk, etc.) implements this interface.
/// </summary>
public interface INotificationChannelProvider
{
    string ChannelName { get; }
    Task<bool> SendAsync(string recipientAddress, string title, string content);
}
