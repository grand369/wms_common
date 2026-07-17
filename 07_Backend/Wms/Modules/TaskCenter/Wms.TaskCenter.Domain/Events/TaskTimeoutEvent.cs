using System;
using Volo.Abp.EventBus;

namespace Wms.TaskCenter.Domain.Events;

/// <summary>
/// DE-033: TaskTimeoutEvent — published when a task exceeds its expected completion time.
/// Subscribers: Notification BC-14 (alert supervisor, may re-assign)
/// </summary>
[EventName("Wms.TaskCenter.TaskTimeout")]
public class TaskTimeoutEvent : EventDataBase
{
    public Guid TaskId { get; }
    public DateTime ExpectedTime { get; }

    public TaskTimeoutEvent(Guid taskId, DateTime expectedTime)
    {
        TaskId = taskId;
        ExpectedTime = expectedTime;
    }
}
