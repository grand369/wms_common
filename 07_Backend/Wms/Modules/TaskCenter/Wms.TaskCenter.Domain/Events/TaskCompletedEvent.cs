using System;
using Volo.Abp.EventBus;

namespace Wms.TaskCenter.Domain.Events;

/// <summary>
/// DE-031: TaskCompletedEvent — published when a task is completed.
/// Subscribers: Outbound/Inbound/Transfer/CycleCount modules for source order status update.
/// </summary>
[EventName("Wms.TaskCenter.TaskCompleted")]
public class TaskCompletedEvent : EventDataBase
{
    public Guid TaskId { get; }
    public int TaskTypeValue { get; }
    public string SourceOrderType { get; }
    public Guid SourceOrderId { get; }
    public DateTime CompletionTime { get; }
    public Guid? AssignedUserId { get; }
    public string? AssignedUserName { get; }
    public string? Remark { get; }

    public TaskCompletedEvent(
        Guid taskId,
        int taskTypeValue,
        string sourceOrderType,
        Guid sourceOrderId,
        DateTime completionTime,
        Guid? assignedUserId = null,
        string? assignedUserName = null,
        string? remark = null)
    {
        TaskId = taskId;
        TaskTypeValue = taskTypeValue;
        SourceOrderType = sourceOrderType;
        SourceOrderId = sourceOrderId;
        CompletionTime = completionTime;
        AssignedUserId = assignedUserId;
        AssignedUserName = assignedUserName;
        Remark = remark;
    }
}
