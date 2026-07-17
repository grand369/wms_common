using System;
using Volo.Abp.EventBus;

namespace Wms.TaskCenter.Domain.Events;

/// <summary>
/// DE-031: TaskCompletedEvent — published when a task is completed.
/// Subscribers: Notification BC-14
/// </summary>
[EventName("Wms.TaskCenter.TaskCompleted")]
public class TaskCompletedEvent : EventDataBase
{
    public Guid TaskId { get; }
    public DateTime CompletionTime { get; }

    public TaskCompletedEvent(Guid taskId, DateTime completionTime)
    {
        TaskId = taskId;
        CompletionTime = completionTime;
    }
}
