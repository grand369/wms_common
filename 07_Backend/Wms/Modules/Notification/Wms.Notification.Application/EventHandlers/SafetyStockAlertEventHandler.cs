using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus;
using Wms.Notification.Domain.Enums;
using Wms.Notification.Domain.Services;
using SafetyStockAlertEvent = Wms.Notification.Domain.Events.SafetyStockAlertEvent;

namespace Wms.Notification.Application.EventHandlers;

/// <summary>
/// Safety Stock Alert Event Handler — sends alert notification when safety stock is breached.
/// </summary>
public class SafetyStockAlertEventHandler : ILocalEventHandler<SafetyStockAlertEvent>, ITransientDependency
{
    private readonly NotificationDomainService _notificationDomainService;

    public SafetyStockAlertEventHandler(NotificationDomainService notificationDomainService)
    {
        _notificationDomainService = notificationDomainService;
    }

    public async Task HandleEventAsync(SafetyStockAlertEvent eventData)
    {
        await _notificationDomainService.CreateAndSendAsync(
            NotificationType.Alert,
            NotificationChannel.Internal,
            "安全库存预警",
            $"物料 {eventData.MaterialCode} 当前可用量 {eventData.CurrentAvailable} 低于安全库存 {eventData.SafetyStockQuantity}",
            Guid.Empty,
            "仓库管理员",
            NotificationPriority.High,
            "SafetyStockAlert",
            "Inventory");
    }
}
