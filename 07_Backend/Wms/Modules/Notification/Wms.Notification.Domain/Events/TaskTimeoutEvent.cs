using Wms.Shared.Domain.Events;

namespace Wms.Notification.Domain.Events;

/// <summary>
/// Task Timeout Event stub — matches Wms.TaskCenter.Domain.Events.TaskTimeoutEvent
/// </summary>
public class TaskTimeoutEvent : EventDataBase
{
    public Guid TaskId { get; set; }
    public DateTime ExpectedTime { get; set; }
}
