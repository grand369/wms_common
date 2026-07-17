using System;

namespace Wms.CycleCount.Domain.Aggregates;

/// <summary>
/// CycleCountItem — sub-entity of CycleCountPlan (AGG-16).
/// Tracks per-item count data: system qty, actual qty, difference.
/// </summary>
public class CycleCountItem : FullAuditedEntity<Guid>
{
    public Guid PlanId { get; private set; }
    public Guid LocationId { get; private set; }
    public string LocationCode { get; private set; }
    public Guid MaterialId { get; private set; }
    public string MaterialCode { get; private set; }
    public string? BatchNumber { get; private set; }
    public decimal SystemQuantity { get; private set; }
    public decimal? ActualQuantity { get; private set; }
    public decimal DifferenceQuantity { get; private set; }

    protected CycleCountItem() { }

    public CycleCountItem(
        Guid id,
        Guid planId,
        Guid locationId,
        string locationCode,
        Guid materialId,
        string materialCode,
        string? batchNumber)
    {
        Id = id;
        PlanId = planId;
        LocationId = locationId;
        LocationCode = locationCode ?? throw new ArgumentNullException(nameof(locationCode));
        MaterialId = materialId;
        MaterialCode = materialCode ?? throw new ArgumentNullException(nameof(materialCode));
        BatchNumber = batchNumber;
        SystemQuantity = 0; // Populated from Inventory when plan starts
        DifferenceQuantity = 0;
    }

    /// <summary>Populate system quantity from inventory</summary>
    internal void SetSystemQuantity(decimal qty) => SystemQuantity = qty;

    /// <summary>Submit actual quantity from PDA scan</summary>
    internal void SubmitActualQuantity(decimal qty)
    {
        ActualQuantity = qty;
        DifferenceQuantity = ActualQuantity.Value - SystemQuantity;
    }

    /// <summary>Reset for recount</summary>
    internal void Recount()
    {
        ActualQuantity = null;
        DifferenceQuantity = 0;
    }
}
