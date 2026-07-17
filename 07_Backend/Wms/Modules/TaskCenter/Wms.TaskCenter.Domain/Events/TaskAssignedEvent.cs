using System;
using Volo.Abp.EventBus;

namespace Wms.TaskCenter.Domain.Events;

/// <summary>
/// DE-030: TaskAssignedEvent — published when a task is assigned to an operator.
/// Subscribers: Notification BC-14 (push to PDA via SignalR)
/// </summary>
[EventName("Wms.TaskCenter.TaskAssigned")]
public class TaskAssignedEvent : EventDataBase
{
    public Guid TaskId { get; }
    public Guid UserId { get; }
    public int TaskTypeValue { get; }

    public TaskAssignedEvent(Guid taskId, Guid userId, int taskTypeValue)
    {
        TaskId = taskId;
        UserId = userId;
        TaskTypeValue = taskTypeValue;
    }
}
