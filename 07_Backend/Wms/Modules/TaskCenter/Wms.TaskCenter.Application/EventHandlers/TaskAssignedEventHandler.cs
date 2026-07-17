using System;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;
using Wms.TaskCenter.Domain.Events;

namespace Wms.TaskCenter.Application.EventHandlers;

/// <summary>
/// DE-030: TaskAssignedEvent handler — placeholder for Notification BC-14.
/// In v1.0, only logs the event. Future: push to PDA via SignalR.
/// </summary>
public class TaskAssignedEventHandler : ILocalEventHandler<TaskAssignedEvent>, ITransientDependency
{
    public async Task HandleEventAsync(TaskAssignedEvent eventData)
    {
        // TODO: v1.1 — push to assigned user's PDA via SignalR
        // "You have been assigned a new task: {eventData.TaskTypeValue}"
    }
}
