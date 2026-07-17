using System;

namespace Wms.LineSide.Domain.Events;

/// <summary>DE-027: Backflush consumed — auto-consumed by production order</summary>
public class BackflushConsumedEvent : EventDataBase
{
    public Guid LineSideWarehouseId { get; }
    public Guid ProductionOrderId { get; }
    public Guid MaterialId { get; }
    public decimal ConsumedQuantity { get; }

    public BackflushConsumedEvent(Guid lineSideWarehouseId, Guid productionOrderId, Guid materialId, decimal consumedQuantity)
    {
        LineSideWarehouseId = lineSideWarehouseId;
        ProductionOrderId = productionOrderId;
        MaterialId = materialId;
        ConsumedQuantity = consumedQuantity;
    }
}
