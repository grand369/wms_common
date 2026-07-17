using System;

namespace Wms.LineSide.Domain.Events;

/// <summary>DE-028: Line side overstock — stock exceeds max quantity (ER-013)</summary>
public class LineSideOverstockEvent : EventDataBase
{
    public Guid LineSideWarehouseId { get; }
    public Guid MaterialId { get; }
    public decimal CurrentQuantity { get; }
    public decimal MaxQuantity { get; }

    public LineSideOverstockEvent(Guid lineSideWarehouseId, Guid materialId, decimal currentQuantity, decimal maxQuantity)
    {
        LineSideWarehouseId = lineSideWarehouseId;
        MaterialId = materialId;
        CurrentQuantity = currentQuantity;
        MaxQuantity = maxQuantity;
    }
}
