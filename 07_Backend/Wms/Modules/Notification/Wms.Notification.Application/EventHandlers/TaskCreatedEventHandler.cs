using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus;
using Wms.Notification.Domain.Enums;
using Wms.Notification.Domain.Services;
using TaskCreatedEvent = Wms.Notification.Domain.Events.TaskCreatedEvent;

namespace Wms.Notification.Application.EventHandlers;

/// <summary>
/// Task Created Event Handler — sends task assignment notification.
/// </summary>
public class TaskCreatedEventHandler : ILocalEventHandler<TaskCreatedEvent>, ITransientDependency
{
    private readonly NotificationDomainService _notificationDomainService;

    public TaskCreatedEventHandler(NotificationDomainService notificationDomainService)
    {
        _notificationDomainService = notificationDomainService;
    }

    public async Task HandleEventAsync(TaskCreatedEvent eventData)
    {
        await _notificationDomainService.CreateAndSendAsync(
            NotificationType.TaskAssignment,
            NotificationChannel.Internal,
            "新任务创建",
            $"新仓库任务已创建，任务ID：{eventData.TaskId}，优先级：{eventData.PriorityValue}",
            Guid.Empty,
            "班组长",
            NotificationPriority.Normal,
            "TaskCreated",
            "TaskCenter");
    }
}
