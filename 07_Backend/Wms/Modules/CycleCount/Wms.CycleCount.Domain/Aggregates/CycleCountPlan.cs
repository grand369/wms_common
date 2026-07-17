using System;
using System.Collections.Generic;
using Wms.CycleCount.Domain.Enums;
using Wms.CycleCount.Domain.Events;

namespace Wms.CycleCount.Domain.Aggregates;

/// <summary>
/// CycleCountPlan Aggregate Root — AGG-16
/// Manages cycle count plans: Full, Cycle (ABC), Spot
/// No explicit state machine — uses CountStatus enum progression
/// </summary>
public class CycleCountPlan : FullAuditedAggregateRoot<Guid>
{
    public string PlanNo { get; private set; }
    public CountMethod CountMethod { get; private set; }
    public CountStatus CountStatus { get; private set; }
    public Guid WarehouseId { get; private set; }
    public string WarehouseCode { get; private set; }
    public DateTime PlannedDate { get; private set; }
    public bool FreezeInventory { get; private set; }
    public decimal DifferenceThreshold { get; private set; }
    public bool BlindCountEnabled { get; private set; }
    public string? Remark { get; private set; }

    public List<CycleCountItem> Items { get; private set; } = new();

    protected CycleCountPlan() { }

    public CycleCountPlan(
        Guid id,
        string planNo,
        CountMethod countMethod,
        Guid warehouseId,
        string warehouseCode,
        DateTime plannedDate,
        bool freezeInventory = true,
        decimal differenceThreshold = 2.0m,
        bool blindCountEnabled = true,
        string? remark = null)
    {
        Id = id;
        PlanNo = planNo ?? throw new ArgumentNullException(nameof(planNo));
        CountMethod = countMethod ?? throw new ArgumentNullException(nameof(countMethod));
        WarehouseId = warehouseId;
        WarehouseCode = warehouseCode ?? throw new ArgumentNullException(nameof(warehouseCode));
        PlannedDate = plannedDate;
        FreezeInventory = freezeInventory;
        DifferenceThreshold = differenceThreshold;
        BlindCountEnabled = blindCountEnabled;
        CountStatus = CountStatus.Planned;
        Remark = remark;
    }

    // ── State Transitions ──────────────────────────────────

    /// <summary>Start counting → Planned → InProgress (BR-032: may freeze inventory)</summary>
    public void StartCounting()
    {
        if (CountStatus != CountStatus.Planned)
            throw new BusinessException("Wms.CycleCount:0101", "Only Planned orders can start counting.");
        CountStatus = CountStatus.InProgress;
    }

    /// <summary>Complete count → InProgress → Completed</summary>
    public void CompleteCounting()
    {
        if (CountStatus != CountStatus.InProgress)
            throw new BusinessException("Wms.CycleCount:0102", "Only InProgress orders can be completed.");
        CountStatus = CountStatus.Completed;
    }

    /// <summary>Close → Completed → Closed</summary>
    public void Close()
    {
        if (CountStatus != CountStatus.Completed)
            throw new BusinessException("Wms.CycleCount:0103", "Only Completed orders can be closed.");
        CountStatus = CountStatus.Closed;
    }

    // ── Item Management ────────────────────────────────────

    public CycleCountItem AddItem(Guid locationId, string locationCode, Guid materialId, string materialCode, string? batchNumber = null)
    {
        var item = new CycleCountItem(Guid.NewGuid(), Id, locationId, locationCode, materialId, materialCode, batchNumber);
        Items.Add(item);
        return item;
    }

    /// <summary>Submit count data for an item (PDA scan result)</summary>
    public void SubmitCountData(Guid itemId, decimal actualQuantity)
    {
        var item = Items.Find(i => i.Id == itemId);
        if (item == null) throw new BusinessException("Wms.CycleCount:0201", "Count item not found.");
        item.SubmitActualQuantity(actualQuantity);
    }

    /// <summary>Recount an item — resets actual quantity</summary>
    public void RecountItem(Guid itemId)
    {
        var item = Items.Find(i => i.Id == itemId);
        if (item == null) throw new BusinessException("Wms.CycleCount:0202", "Count item not found.");
        item.Recount();
    }

    /// <summary>Check if any item has difference over threshold — DE-025</summary>
    public void CheckDifferenceOverThreshold()
    {
        foreach (var item in Items.Where(i => i.ActualQuantity.HasValue))
        {
            var diff = Math.Abs(item.DifferenceQuantity);
            var thresholdQty = item.SystemQuantity * DifferenceThreshold / 100;
            if (diff > thresholdQty)
            {
                AddLocalEvent(new CountDifferenceOverThresholdEvent(Id, item.MaterialId, diff, DifferenceThreshold));
            }
        }
    }
}
