using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus;
using Wms.Notification.Domain.Enums;
using Wms.Notification.Domain.Services;
using TaskTimeoutEvent = Wms.Notification.Domain.Events.TaskTimeoutEvent;

namespace Wms.Notification.Application.EventHandlers;

/// <summary>
/// Task Timeout Event Handler — sends timeout alert when task exceeds expected completion time.
/// </summary>
public class TaskTimeoutEventHandler : ILocalEventHandler<TaskTimeoutEvent>, ITransientDependency
{
    private readonly NotificationDomainService _notificationDomainService;

    public TaskTimeoutEventHandler(NotificationDomainService notificationDomainService)
    {
        _notificationDomainService = notificationDomainService;
    }

    public async Task HandleEventAsync(TaskTimeoutEvent eventData)
    {
        await _notificationDomainService.CreateAndSendAsync(
            NotificationType.Alert,
            NotificationChannel.Internal,
            "任务超时告警",
            $"任务 {eventData.TaskId} 已超时，预期完成时间：{eventData.ExpectedTime:yyyy-MM-dd HH:mm}",
            Guid.Empty,
            "班组长",
            NotificationPriority.High,
            "TaskTimeout",
            "TaskCenter");
    }
}
