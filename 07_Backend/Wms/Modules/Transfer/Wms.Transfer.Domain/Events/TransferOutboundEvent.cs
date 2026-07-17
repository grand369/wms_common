using System;
using System.Collections.Generic;
using Wms.Shared.Domain.Enums;

namespace Wms.Transfer.Domain.Events;

/// <summary>
/// DE-021: TransferOutboundEvent — published when source warehouse outbound is confirmed.
/// Subscribers: Inventory (decrease source), TaskCenter (create outbound task)
/// </summary>
public class TransferOutboundEvent : EventDataBase
{
    public Guid OrderId { get; }
    public Guid SourceWarehouseId { get; }
    public List<TransferLineData> Lines { get; }

    public TransferOutboundEvent(Guid orderId, Guid sourceWarehouseId, List<Domain.Aggregates.TransferLine> lines)
    {
        OrderId = orderId;
        SourceWarehouseId = sourceWarehouseId;
        Lines = lines.Select(l => new TransferLineData
        {
            MaterialId = l.MaterialId,
            TransferQuantity = l.TransferQuantity,
            OutboundConfirmedQuantity = l.OutboundConfirmedQuantity
        }).ToList();
    }
}

public class TransferLineData
{
    public Guid MaterialId { get; set; }
    public decimal TransferQuantity { get; set; }
    public decimal OutboundConfirmedQuantity { get; set; }
    public decimal InboundConfirmedQuantity { get; set; }
}
