using System;
using Volo.Abp.EventBus;

namespace Wms.TaskCenter.Domain.Events;

/// <summary>
/// DE-032: TaskSuspendedEvent — published when a task is suspended with a reason.
/// Subscribers: Notification BC-14 (alert warehouse supervisor)
/// </summary>
[EventName("Wms.TaskCenter.TaskSuspended")]
public class TaskSuspendedEvent : EventDataBase
{
    public Guid TaskId { get; }
    public string Reason { get; }

    public TaskSuspendedEvent(Guid taskId, string reason)
    {
        TaskId = taskId;
        Reason = reason;
    }
}
