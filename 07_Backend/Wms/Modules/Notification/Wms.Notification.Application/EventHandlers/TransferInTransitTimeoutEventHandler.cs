using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus;
using Wms.Notification.Domain.Enums;
using Wms.Notification.Domain.Services;
using TransferInTransitTimeoutEvent = Wms.Notification.Domain.Events.TransferInTransitTimeoutEvent;

namespace Wms.Notification.Application.EventHandlers;

/// <summary>
/// Transfer In Transit Timeout Event Handler — sends timeout alert for transfer orders.
/// </summary>
public class TransferInTransitTimeoutEventHandler : ILocalEventHandler<TransferInTransitTimeoutEvent>, ITransientDependency
{
    private readonly NotificationDomainService _notificationDomainService;

    public TransferInTransitTimeoutEventHandler(NotificationDomainService notificationDomainService)
    {
        _notificationDomainService = notificationDomainService;
    }

    public async Task HandleEventAsync(TransferInTransitTimeoutEvent eventData)
    {
        await _notificationDomainService.CreateAndSendAsync(
            NotificationType.Alert,
            NotificationChannel.Internal,
            "在途超时告警",
            $"调拨单 {eventData.OrderId} 在途超时（EWO-011），源仓：{eventData.SourceWarehouseId}，目标仓：{eventData.TargetWarehouseId}",
            Guid.Empty,
            "仓库管理员",
            NotificationPriority.High,
            "TransferInTransitTimeout",
            "Transfer");
    }
}
