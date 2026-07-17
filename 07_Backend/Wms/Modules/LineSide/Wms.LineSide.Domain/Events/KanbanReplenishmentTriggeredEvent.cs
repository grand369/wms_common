using System;

namespace Wms.LineSide.Domain.Events;

/// <summary>DE-026: Kanban replenishment triggered when stock below min (BR-029)</summary>
public class KanbanReplenishmentTriggeredEvent : EventDataBase
{
    public Guid LineSideWarehouseId { get; }
    public Guid MaterialId { get; }
    public decimal ReplenishmentQuantity { get; }

    public KanbanReplenishmentTriggeredEvent(Guid lineSideWarehouseId, Guid materialId, decimal replenishmentQuantity)
    {
        LineSideWarehouseId = lineSideWarehouseId;
        MaterialId = materialId;
        ReplenishmentQuantity = replenishmentQuantity;
    }
}
