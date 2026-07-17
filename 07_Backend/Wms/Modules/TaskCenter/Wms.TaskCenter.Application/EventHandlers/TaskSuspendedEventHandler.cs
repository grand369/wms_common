using System;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;
using Wms.TaskCenter.Domain.Events;

namespace Wms.TaskCenter.Application.EventHandlers;

/// <summary>
/// DE-032: TaskSuspendedEvent handler — placeholder for Notification BC-14.
/// In v1.0, only logs the event. Future: alert warehouse supervisor.
/// </summary>
public class TaskSuspendedEventHandler : ILocalEventHandler<TaskSuspendedEvent>, ITransientDependency
{
    public async Task HandleEventAsync(TaskSuspendedEvent eventData)
    {
        // TODO: v1.1 — alert warehouse supervisor
        // "Task {eventData.TaskId} suspended: {eventData.Reason}"
    }
}
