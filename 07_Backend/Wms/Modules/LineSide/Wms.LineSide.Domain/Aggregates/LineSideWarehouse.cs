using System;
using System.Collections.Generic;
using Wms.LineSide.Domain.Enums;
using Wms.LineSide.Domain.Events;

namespace Wms.LineSide.Domain.Aggregates;

/// <summary>
/// LineSideWarehouse Aggregate Root — AGG-18
/// Manages line-side warehouses bound to production lines + work stations.
/// Kanban parameters (min/max) trigger replenishment when stock drops below min.
/// </summary>
public class LineSideWarehouse : FullAuditedAggregateRoot<Guid>
{
    public string LineSideWarehouseCode { get; private set; }
    public string LineSideWarehouseName { get; private set; }
    public Guid WarehouseId { get; private set; }
    public string WarehouseCode { get; private set; }
    public Guid ProductionLineId { get; private set; }
    public string ProductionLineName { get; private set; }
    public Guid? WorkStationId { get; private set; }
    public ConsumptionMode ConsumptionMode { get; private set; }

    public List<LineSideKanbanItem> KanbanItems { get; private set; } = new();

    protected LineSideWarehouse() { }

    public LineSideWarehouse(
        Guid id,
        string code,
        string name,
        Guid warehouseId,
        string warehouseCode,
        Guid productionLineId,
        string productionLineName,
        Guid? workStationId = null,
        ConsumptionMode consumptionMode = null)
    {
        Id = id;
        LineSideWarehouseCode = code ?? throw new ArgumentNullException(nameof(code));
        LineSideWarehouseName = name ?? throw new ArgumentNullException(nameof(name));
        WarehouseId = warehouseId;
        WarehouseCode = warehouseCode ?? throw new ArgumentNullException(nameof(warehouseCode));
        ProductionLineId = productionLineId;
        ProductionLineName = productionLineName ?? throw new ArgumentNullException(nameof(productionLineName));
        WorkStationId = workStationId;
        ConsumptionMode = consumptionMode ?? ConsumptionMode.Scan;
    }

    // ── Kanban Management ──────────────────────────────────

    public LineSideKanbanItem AddKanbanItem(Guid materialId, string materialCode, decimal minQuantity, decimal maxQuantity)
    {
        var item = new LineSideKanbanItem(Guid.NewGuid(), Id, materialId, materialCode, minQuantity, maxQuantity);
        KanbanItems.Add(item);
        return item;
    }

    /// <summary>BR-029: Check if any kanban item is below min → trigger replenishment (DE-026)</summary>
    public void CheckKanbanThresholds()
    {
        foreach (var item in KanbanItems)
        {
            if (item.CurrentQuantity < item.MinQuantity)
            {
                var replenishQty = item.MaxQuantity - item.CurrentQuantity;
                AddLocalEvent(new KanbanReplenishmentTriggeredEvent(Id, item.MaterialId, replenishQty));
            }
            else if (item.CurrentQuantity > item.MaxQuantity)
            {
                AddLocalEvent(new LineSideOverstockEvent(Id, item.MaterialId, item.CurrentQuantity, item.MaxQuantity));
            }
        }
    }

    /// <summary>Consume material in backflush mode (DE-027)</summary>
    public void BackflushConsume(Guid productionOrderId, Guid materialId, decimal consumeQty)
    {
        var item = KanbanItems.Find(k => k.MaterialId == materialId);
        if (item == null) throw new BusinessException("Wms.LineSide:0101", $"Kanban item for material {materialId} not found.");
        if (item.CurrentQuantity < consumeQty) throw new BusinessException("Wms.LineSide:0102", "Insufficient line-side stock for backflush consumption.");

        item.Consume(consumeQty);
        AddLocalEvent(new BackflushConsumedEvent(Id, productionOrderId, materialId, consumeQty));
        CheckKanbanThresholds();
    }

    /// <summary>Receive replenishment — increase kanban item quantity</summary>
    public void ReceiveReplenishment(Guid materialId, decimal qty)
    {
        var item = KanbanItems.Find(k => k.MaterialId == materialId);
        if (item == null) throw new BusinessException("Wms.LineSide:0103", $"Kanban item for material {materialId} not found.");
        item.Receive(qty);
    }
}
