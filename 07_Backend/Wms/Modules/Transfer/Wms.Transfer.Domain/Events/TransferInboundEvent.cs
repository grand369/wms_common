using System;
using System.Collections.Generic;

namespace Wms.Transfer.Domain.Events;

/// <summary>
/// DE-022: TransferInboundEvent — published when target warehouse inbound is confirmed.
/// Subscribers: Inventory (increase target + clear in-transit), TaskCenter (create inbound task)
/// </summary>
public class TransferInboundEvent : EventDataBase
{
    public Guid OrderId { get; }
    public Guid TargetWarehouseId { get; }
    public List<TransferLineData> Lines { get; }

    public TransferInboundEvent(Guid orderId, Guid targetWarehouseId, List<Domain.Aggregates.TransferLine> lines)
    {
        OrderId = orderId;
        TargetWarehouseId = targetWarehouseId;
        Lines = lines.Select(l => new TransferLineData
        {
            MaterialId = l.MaterialId,
            TransferQuantity = l.TransferQuantity,
            InboundConfirmedQuantity = l.InboundConfirmedQuantity
        }).ToList();
    }
}
