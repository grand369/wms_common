using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus;
using Wms.Notification.Domain.Enums;
using Wms.Notification.Domain.Services;
using ZeroInventoryAlertEvent = Wms.Notification.Domain.Events.ZeroInventoryAlertEvent;

namespace Wms.Notification.Application.EventHandlers;

/// <summary>
/// Zero Inventory Alert Event Handler — sends alert when inventory reaches zero with pending demand.
/// </summary>
public class ZeroInventoryAlertEventHandler : ILocalEventHandler<ZeroInventoryAlertEvent>, ITransientDependency
{
    private readonly NotificationDomainService _notificationDomainService;

    public ZeroInventoryAlertEventHandler(NotificationDomainService notificationDomainService)
    {
        _notificationDomainService = notificationDomainService;
    }

    public async Task HandleEventAsync(ZeroInventoryAlertEvent eventData)
    {
        await _notificationDomainService.CreateAndSendAsync(
            NotificationType.Alert,
            NotificationChannel.Internal,
            "零库存告警",
            $"物料 {eventData.MaterialCode} 库存为零，待处理需求 {eventData.PendingDemand}",
            Guid.Empty,
            "仓库管理员",
            NotificationPriority.Emergency,
            "ZeroInventoryAlert",
            "Inventory");
    }
}
