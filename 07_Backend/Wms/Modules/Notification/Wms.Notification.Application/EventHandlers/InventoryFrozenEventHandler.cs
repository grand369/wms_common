using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus;
using Wms.Notification.Domain.Enums;
using Wms.Notification.Domain.Services;
using InventoryFrozenEvent = Wms.Notification.Domain.Events.InventoryFrozenEvent;

namespace Wms.Notification.Application.EventHandlers;

/// <summary>
/// Inventory Frozen Event Handler — sends notification when inventory is frozen.
/// </summary>
public class InventoryFrozenEventHandler : ILocalEventHandler<InventoryFrozenEvent>, ITransientDependency
{
    private readonly NotificationDomainService _notificationDomainService;

    public InventoryFrozenEventHandler(NotificationDomainService notificationDomainService)
    {
        _notificationDomainService = notificationDomainService;
    }

    public async Task HandleEventAsync(InventoryFrozenEvent eventData)
    {
        await _notificationDomainService.CreateAndSendAsync(
            NotificationType.System,
            NotificationChannel.Internal,
            "库存冻结通知",
            $"冻结单 {eventData.FreezeOrderNo} 已冻结库存 {eventData.FrozenQuantity}，原因：{eventData.FreezeReason}",
            Guid.Empty,
            "仓库管理员",
            NotificationPriority.Normal,
            "InventoryFrozen",
            "Inventory");
    }
}
