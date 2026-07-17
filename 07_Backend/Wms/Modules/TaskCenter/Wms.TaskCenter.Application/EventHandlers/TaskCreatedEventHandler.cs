using System;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;
using Wms.TaskCenter.Domain.Events;

namespace Wms.TaskCenter.Application.EventHandlers;

/// <summary>
/// DE-029: TaskCreatedEvent handler — placeholder for Notification BC-14.
/// In v1.0, only logs the event. Future: push notification via SignalR/email.
/// </summary>
public class TaskCreatedEventHandler : ILocalEventHandler<TaskCreatedEvent>, ITransientDependency
{
    public async Task HandleEventAsync(TaskCreatedEvent eventData)
    {
        // TODO: v1.1 — integrate with Notification module (BC-14)
        // Push to supervisor dashboard: "New task created: {eventData.TaskTypeValue} for order {eventData.SourceOrderId}"
    }
}
