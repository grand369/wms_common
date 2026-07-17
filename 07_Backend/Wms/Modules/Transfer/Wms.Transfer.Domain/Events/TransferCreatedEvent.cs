using System;

namespace Wms.Transfer.Domain.Events;

/// <summary>
/// TransferCreatedEvent — local event published when a new transfer order is created.
/// </summary>
public class TransferCreatedEvent : EventDataBase
{
    public Guid OrderId { get; }
    public string OrderNo { get; }
    public int TransferTypeValue { get; }
    public Guid SourceWarehouseId { get; }
    public Guid TargetWarehouseId { get; }

    public TransferCreatedEvent(
        Guid orderId,
        string orderNo,
        Wms.Shared.Domain.Enums.TransferType transferType,
        Guid sourceWarehouseId,
        Guid targetWarehouseId)
    {
        OrderId = orderId;
        OrderNo = orderNo;
        TransferTypeValue = transferType.Value;
        SourceWarehouseId = sourceWarehouseId;
        TargetWarehouseId = targetWarehouseId;
    }
}
