using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus;
using Wms.Notification.Domain.Enums;
using Wms.Notification.Domain.Services;
using LineSideOverstockEvent = Wms.Notification.Domain.Events.LineSideOverstockEvent;

namespace Wms.Notification.Application.EventHandlers;

/// <summary>
/// Line Side Overstock Event Handler — sends overstock alert for line side warehouses.
/// </summary>
public class LineSideOverstockEventHandler : ILocalEventHandler<LineSideOverstockEvent>, ITransientDependency
{
    private readonly NotificationDomainService _notificationDomainService;

    public LineSideOverstockEventHandler(NotificationDomainService notificationDomainService)
    {
        _notificationDomainService = notificationDomainService;
    }

    public async Task HandleEventAsync(LineSideOverstockEvent eventData)
    {
        await _notificationDomainService.CreateAndSendAsync(
            NotificationType.Alert,
            NotificationChannel.Internal,
            "线边仓超量告警",
            $"线边仓 {eventData.LineSideWarehouseId} 物料 {eventData.MaterialId} 当前库存 {eventData.CurrentQuantity} 超过上限 {eventData.MaxQuantity}",
            Guid.Empty,
            "线边仓管理员",
            NotificationPriority.High,
            "LineSideOverstock",
            "LineSide");
    }
}
