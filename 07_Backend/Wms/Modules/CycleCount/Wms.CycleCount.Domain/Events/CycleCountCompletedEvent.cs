using System;

namespace Wms.CycleCount.Domain.Events;

/// <summary>
/// DE-024: CycleCountCompletedEvent — published when count result is finalized.
/// Subscribers: Inventory (adjust stock), Notification (alert)
/// </summary>
public class CycleCountCompletedEvent : EventDataBase
{
    public Guid PlanId { get; }
    public Guid LocationId { get; }
    public Guid MaterialId { get; }
    public decimal SystemQuantity { get; }
    public decimal ActualQuantity { get; }
    public decimal DifferenceQuantity { get; }

    public CycleCountCompletedEvent(
        Guid planId, Guid locationId, Guid materialId,
        decimal systemQuantity, decimal actualQuantity, decimal differenceQuantity)
    {
        PlanId = planId;
        LocationId = locationId;
        MaterialId = materialId;
        SystemQuantity = systemQuantity;
        ActualQuantity = actualQuantity;
        DifferenceQuantity = differenceQuantity;
    }
}
