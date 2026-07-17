using Wms.Shared.Domain.Events;

namespace Wms.Notification.Domain.Events;

/// <summary>
/// Task Assigned Event stub — matches Wms.TaskCenter.Domain.Events.TaskAssignedEvent
/// </summary>
public class TaskAssignedEvent : EventDataBase
{
    public Guid TaskId { get; set; }
    public Guid UserId { get; set; }
    public int TaskTypeValue { get; set; }
}
