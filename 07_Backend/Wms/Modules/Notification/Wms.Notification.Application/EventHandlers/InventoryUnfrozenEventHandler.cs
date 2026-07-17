using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus;
using Wms.Notification.Domain.Enums;
using Wms.Notification.Domain.Services;
using InventoryUnfrozenEvent = Wms.Notification.Domain.Events.InventoryUnfrozenEvent;

namespace Wms.Notification.Application.EventHandlers;

/// <summary>
/// Inventory Unfrozen Event Handler — sends notification when inventory is unfrozen/released.
/// </summary>
public class InventoryUnfrozenEventHandler : ILocalEventHandler<InventoryUnfrozenEvent>, ITransientDependency
{
    private readonly NotificationDomainService _notificationDomainService;

    public InventoryUnfrozenEventHandler(NotificationDomainService notificationDomainService)
    {
        _notificationDomainService = notificationDomainService;
    }

    public async Task HandleEventAsync(InventoryUnfrozenEvent eventData)
    {
        await _notificationDomainService.CreateAndSendAsync(
            NotificationType.System,
            NotificationChannel.Internal,
            "库存解冻通知",
            $"冻结单 {eventData.FreezeOrderNo} 已解冻库存 {eventData.UnfrozenQuantity}，原因：{eventData.ReleaseReason}",
            Guid.Empty,
            "仓库管理员",
            NotificationPriority.Normal,
            "InventoryUnfrozen",
            "Inventory");
    }
}
