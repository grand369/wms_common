using System;
using Wms.CycleCount.Domain.Enums;
using Wms.CycleCount.Domain.Events;

namespace Wms.CycleCount.Domain.Aggregates;

/// <summary>
/// CycleCountResult — AGG-17, independent aggregate root.
/// Stores final count results with difference analysis.
/// </summary>
public class CycleCountResult : FullAuditedAggregateRoot<Guid>
{
    public Guid PlanId { get; private set; }
    public Guid LocationId { get; private set; }
    public string LocationCode { get; private set; }
    public Guid MaterialId { get; private set; }
    public string MaterialCode { get; private set; }
    public decimal SystemQuantity { get; private set; }
    public decimal ActualQuantity { get; private set; }
    public decimal DifferenceQuantity { get; private set; }
    public decimal DifferenceAmount { get; private set; }
    public bool BlindCountFlag { get; private set; }
    public int ResultStatusValue { get; private set; }

    protected CycleCountResult() { }

    public CycleCountResult(
        Guid id,
        Guid planId,
        Guid locationId,
        string locationCode,
        Guid materialId,
        string materialCode,
        decimal systemQuantity,
        decimal actualQuantity,
        decimal differenceAmount,
        bool blindCountFlag)
    {
        Id = id;
        PlanId = planId;
        LocationId = locationId;
        LocationCode = locationCode;
        MaterialId = materialId;
        MaterialCode = materialCode;
        SystemQuantity = systemQuantity;
        ActualQuantity = actualQuantity;
        DifferenceQuantity = actualQuantity - systemQuantity;
        DifferenceAmount = differenceAmount;
        BlindCountFlag = blindCountFlag;
        ResultStatusValue = 0; // Pending

        AddLocalEvent(new CycleCountCompletedEvent(planId, locationId, materialId, systemQuantity, actualQuantity, DifferenceQuantity));
    }
}
