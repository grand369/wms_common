using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus;
using Wms.Notification.Domain.Enums;
using Wms.Notification.Domain.Services;
using TaskAssignedEvent = Wms.Notification.Domain.Events.TaskAssignedEvent;

namespace Wms.Notification.Application.EventHandlers;

/// <summary>
/// Task Assigned Event Handler — sends assignment notification to the assigned user.
/// </summary>
public class TaskAssignedEventHandler : ILocalEventHandler<TaskAssignedEvent>, ITransientDependency
{
    private readonly NotificationDomainService _notificationDomainService;

    public TaskAssignedEventHandler(NotificationDomainService notificationDomainService)
    {
        _notificationDomainService = notificationDomainService;
    }

    public async Task HandleEventAsync(TaskAssignedEvent eventData)
    {
        await _notificationDomainService.CreateAndSendAsync(
            NotificationType.TaskAssignment,
            NotificationChannel.Internal,
            "任务分配通知",
            $"任务 {eventData.TaskId} 已分配给您，请及时处理",
            eventData.UserId,
            "操作员",
            NotificationPriority.Normal,
            "TaskAssigned",
            "TaskCenter");
    }
}
