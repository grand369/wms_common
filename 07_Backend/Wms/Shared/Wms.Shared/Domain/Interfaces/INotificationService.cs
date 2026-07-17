namespace Wms.Shared.Domain.Interfaces;

/// <summary>
/// INotificationService — Shared Kernel interface for cross-module notification sending.
/// Defined in Shared Kernel so that other modules can inject this interface via DI
/// to send notifications without referencing Notification.Domain directly.
/// </summary>
public interface INotificationService
{
    /// <summary>
    /// Send a notification to a single recipient.
    /// </summary>
    Task SendNotificationAsync(
        int notificationTypeValue,
        int channelValue,
        string title,
        string content,
        Guid recipientId,
        string recipientName);

    /// <summary>
    /// Send a notification to multiple recipients in batch.
    /// </summary>
    Task SendBatchNotificationAsync(
        int notificationTypeValue,
        int channelValue,
        string title,
        string content,
        List<Guid> recipientIds);
}
