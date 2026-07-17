using Wms.Shared.Domain.Events;

namespace Wms.Notification.Domain.Events;

/// <summary>
/// Transfer In Transit Timeout Event stub — matches Wms.Transfer.Domain.Events.TransferInTransitTimeoutEvent
/// </summary>
public class TransferInTransitTimeoutEvent : EventDataBase
{
    public Guid OrderId { get; set; }
    public Guid SourceWarehouseId { get; set; }
    public Guid TargetWarehouseId { get; set; }
}
