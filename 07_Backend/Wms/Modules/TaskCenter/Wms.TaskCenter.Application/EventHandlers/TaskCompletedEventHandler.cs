using System;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;
using Wms.TaskCenter.Domain.Events;

namespace Wms.TaskCenter.Application.EventHandlers;

/// <summary>
/// DE-031: TaskCompletedEvent handler — placeholder for Notification + ERP callback.
/// In v1.0, only logs the event. Future: notify upstream modules (Inbound/Outbound) that task is done.
/// </summary>
public class TaskCompletedEventHandler : ILocalEventHandler<TaskCompletedEvent>, ITransientDependency
{
    public async Task HandleEventAsync(TaskCompletedEvent eventData)
    {
        // TODO: v1.1 — notify upstream Inbound/Outbound that their task is completed
        // ERP callback placeholder
    }
}
