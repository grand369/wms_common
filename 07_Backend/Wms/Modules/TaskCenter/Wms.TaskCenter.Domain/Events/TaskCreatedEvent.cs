using System;
using Volo.Abp.EventBus;

namespace Wms.TaskCenter.Domain.Events;

/// <summary>
/// DE-029: TaskCreatedEvent — published when a new warehouse task is created.
/// Subscribers: Notification BC-14
/// </summary>
[EventName("Wms.TaskCenter.TaskCreated")]
public class TaskCreatedEvent : EventDataBase
{
    public Guid TaskId { get; }
    public int TaskTypeValue { get; }
    public int PriorityValue { get; }
    public Guid SourceOrderId { get; }

    public TaskCreatedEvent(Guid taskId, int taskTypeValue, int priorityValue, Guid sourceOrderId)
    {
        TaskId = taskId;
        TaskTypeValue = taskTypeValue;
        PriorityValue = priorityValue;
        SourceOrderId = sourceOrderId;
    }
}
