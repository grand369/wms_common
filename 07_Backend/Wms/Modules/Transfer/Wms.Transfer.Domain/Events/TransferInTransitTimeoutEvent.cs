using System;

namespace Wms.Transfer.Domain.Events;

/// <summary>
/// DE-023: TransferInTransitTimeoutEvent — published when in-transit exceeds expected duration (ER-011).
/// Subscribers: Notification (alert supervisor)
/// </summary>
public class TransferInTransitTimeoutEvent : EventDataBase
{
    public Guid OrderId { get; }
    public Guid SourceWarehouseId { get; }
    public Guid TargetWarehouseId { get; }

    public TransferInTransitTimeoutEvent(Guid orderId, Guid sourceWarehouseId, Guid targetWarehouseId)
    {
        OrderId = orderId;
        SourceWarehouseId = sourceWarehouseId;
        TargetWarehouseId = targetWarehouseId;
    }
}
