using System;

namespace Wms.LineSide.Domain.Aggregates;

/// <summary>
/// LineSideKanbanItem — sub-entity of LineSideWarehouse (AGG-18).
/// Kanban parameters: MinQuantity (replenishment trigger) / MaxQuantity (target after replenishment)
/// </summary>
public class LineSideKanbanItem : FullAuditedEntity<Guid>
{
    public Guid LineSideWarehouseId { get; private set; }
    public Guid MaterialId { get; private set; }
    public string MaterialCode { get; private set; }
    public decimal MinQuantity { get; private set; }
    public decimal MaxQuantity { get; private set; }
    public decimal CurrentQuantity { get; private set; }

    protected LineSideKanbanItem() { }

    public LineSideKanbanItem(Guid id, Guid lineSideWarehouseId, Guid materialId, string materialCode, decimal minQuantity, decimal maxQuantity)
    {
        Id = id;
        LineSideWarehouseId = lineSideWarehouseId;
        MaterialId = materialId;
        MaterialCode = materialCode ?? throw new ArgumentNullException(nameof(materialCode));
        MinQuantity = minQuantity;
        MaxQuantity = maxQuantity;
        CurrentQuantity = 0;
    }

    internal void Consume(decimal qty)
    {
        if (qty > CurrentQuantity) throw new BusinessException("Wms.LineSide:0201", "Consume quantity exceeds current stock.");
        CurrentQuantity -= qty;
    }

    internal void Receive(decimal qty) => CurrentQuantity += qty;
}
