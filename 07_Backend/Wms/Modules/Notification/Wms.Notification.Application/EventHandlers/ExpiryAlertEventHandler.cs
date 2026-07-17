using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus;
using Wms.Notification.Domain.Enums;
using Wms.Notification.Domain.Services;
using ExpiryAlertEvent = Wms.Notification.Domain.Events.ExpiryAlertEvent;

namespace Wms.Notification.Application.EventHandlers;

/// <summary>
/// Expiry Alert Event Handler — sends alert when inventory items are near expiry.
/// </summary>
public class ExpiryAlertEventHandler : ILocalEventHandler<ExpiryAlertEvent>, ITransientDependency
{
    private readonly NotificationDomainService _notificationDomainService;

    public ExpiryAlertEventHandler(NotificationDomainService notificationDomainService)
    {
        _notificationDomainService = notificationDomainService;
    }

    public async Task HandleEventAsync(ExpiryAlertEvent eventData)
    {
        await _notificationDomainService.CreateAndSendAsync(
            NotificationType.Alert,
            NotificationChannel.Internal,
            "效期预警",
            $"物料 {eventData.MaterialCode} 批次 {eventData.BatchNumber} 将于 {eventData.ExpiryDate:yyyy-MM-dd} 到期，剩余 {eventData.DaysLeft} 天",
            Guid.Empty,
            "仓库管理员",
            NotificationPriority.High,
            "ExpiryAlert",
            "Inventory");
    }
}
