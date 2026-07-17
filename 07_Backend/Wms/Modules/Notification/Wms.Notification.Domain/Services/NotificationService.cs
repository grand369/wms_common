using Volo.Abp.Domain.Services;
using Wms.Notification.Domain.Aggregates;
using Wms.Notification.Domain.Enums;
using Wms.Notification.Domain.Repositories;
using Wms.Notification.Domain.Services;
using Wms.Shared.Domain.Interfaces;

namespace Wms.Notification.Domain.Services;

/// <summary>
/// NotificationService — implements INotificationService from Shared Kernel.
/// Provides cross-module notification sending capability.
/// </summary>
public class NotificationService : DomainService, INotificationService
{
    private readonly NotificationDomainService _notificationDomainService;

    public NotificationService(NotificationDomainService notificationDomainService)
    {
        _notificationDomainService = notificationDomainService;
    }

    public async Task SendNotificationAsync(
        int notificationTypeValue,
        int channelValue,
        string title,
        string content,
        Guid recipientId,
        string recipientName)
    {
        var notificationType = NotificationType.FromValue(notificationTypeValue);
        var channel = NotificationChannel.FromValue(channelValue);

        await _notificationDomainService.CreateAndSendAsync(
            notificationType,
            channel,
            title,
            content,
            recipientId,
            recipientName);
    }

    public async Task SendBatchNotificationAsync(
        int notificationTypeValue,
        int channelValue,
        string title,
        string content,
        List<Guid> recipientIds)
    {
        var notificationType = NotificationType.FromValue(notificationTypeValue);
        var channel = NotificationChannel.FromValue(channelValue);

        foreach (var recipientId in recipientIds)
        {
            await _notificationDomainService.CreateAndSendAsync(
                notificationType,
                channel,
                title,
                content,
                recipientId,
                "BatchRecipient");
        }
    }
}
