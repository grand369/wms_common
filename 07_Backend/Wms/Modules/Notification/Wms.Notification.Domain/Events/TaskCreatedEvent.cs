using Wms.Shared.Domain.Events;

namespace Wms.Notification.Domain.Events;

/// <summary>
/// Task Created Event stub — matches Wms.TaskCenter.Domain.Events.TaskCreatedEvent
/// </summary>
public class TaskCreatedEvent : EventDataBase
{
    public Guid TaskId { get; set; }
    public int TaskTypeValue { get; set; }
    public int PriorityValue { get; set; }
    public Guid SourceOrderId { get; set; }
}
