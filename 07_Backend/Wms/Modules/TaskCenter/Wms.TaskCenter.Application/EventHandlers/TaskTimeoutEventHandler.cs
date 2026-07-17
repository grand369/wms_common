using System;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;
using Wms.TaskCenter.Domain.Events;

namespace Wms.TaskCenter.Application.EventHandlers;

/// <summary>
/// DE-033: TaskTimeoutEvent handler — placeholder for Notification BC-14.
/// In v1.0, only logs the event. Future: send timeout warning via SignalR + email.
/// </summary>
public class TaskTimeoutEventHandler : ILocalEventHandler<TaskTimeoutEvent>, ITransientDependency
{
    public async Task HandleEventAsync(TaskTimeoutEvent eventData)
    {
        // TODO: v1.1 — timeout warning notification
        // "Task {eventData.TaskId} has exceeded expected completion time {eventData.ExpectedTime}"
    }
}
