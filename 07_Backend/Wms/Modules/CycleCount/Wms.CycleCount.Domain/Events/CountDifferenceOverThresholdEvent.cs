using System;

namespace Wms.CycleCount.Domain.Events;

/// <summary>
/// DE-025: CountDifferenceOverThresholdEvent — published when difference exceeds threshold (ER-010).
/// Subscribers: Notification (alert supervisor), Workflow (trigger approval)
/// </summary>
public class CountDifferenceOverThresholdEvent : EventDataBase
{
    public Guid PlanId { get; }
    public Guid MaterialId { get; }
    public decimal DifferenceAmount { get; }
    public decimal ThresholdPercent { get; }

    public CountDifferenceOverThresholdEvent(Guid planId, Guid materialId, decimal differenceAmount, decimal thresholdPercent)
    {
        PlanId = planId;
        MaterialId = materialId;
        DifferenceAmount = differenceAmount;
        ThresholdPercent = thresholdPercent;
    }
}
