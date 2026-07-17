using Volo.Abp.Domain.Entities.Auditing;
using Wms.Outbound.Domain.Enums;

namespace Wms.Outbound.Domain.Aggregates;

/// <summary>
/// OutboundLine Child Entity (ENT-09a) — nested within OutboundOrder aggregate.
/// Represents a single material line in an outbound order.
/// Inherits FullAuditedEntity<Guid> (not aggregate root — lifecycle bound to parent).
/// (AGG-13, Phase 3 DDD Design)
/// </summary>
public class OutboundLine : FullAuditedEntity<Guid>
{
    /// <summary>Parent outbound order ID — foreign key.</summary>
    public Guid OutboundOrderId { get; private set; }

    /// <summary>Line number — sequential within the order.</summary>
    public int LineNo { get; private set; }

    /// <summary>Material ID.</summary>
    public Guid MaterialId { get; private set; }

    /// <summary>Material code — redundant.</summary>
    public string MaterialCode { get; private set; }

    /// <summary>Material name — redundant.</summary>
    public string MaterialName { get; private set; }

    /// <summary>Required quantity — demand quantity from requisition/BOM.</summary>
    public decimal RequiredQuantity { get; private set; }

    /// <summary>Allocated quantity — quantity reserved from inventory (default 0).</summary>
    public decimal AllocatedQuantity { get; private set; }

    /// <summary>Picked quantity — quantity physically picked (default 0).</summary>
    public decimal PickedQuantity { get; private set; }

    /// <summary>Shipped quantity — quantity actually shipped out (default 0).</summary>
    public decimal ShippedQuantity { get; private set; }

    /// <summary>Picking location ID — system recommended or manually specified.</summary>
    public Guid? PickingLocationId { get; private set; }

    /// <summary>Picking location code — redundant.</summary>
    public string? PickingLocationCode { get; private set; }

    /// <summary>Issue strategy — FIFO/FEFO/FMFO/Manual.</summary>
    public IssueStrategyType IssueStrategy { get; private set; }

    /// <summary>Batch number — nullable.</summary>
    public string? BatchNumber { get; private set; }

    /// <summary>Remark.</summary>
    public string? Remark { get; private set; }

    private OutboundLine() { }

    public OutboundLine(
        Guid id,
        Guid outboundOrderId,
        int lineNo,
        Guid materialId,
        string materialCode,
        string materialName,
        decimal requiredQuantity,
        IssueStrategyType issueStrategy,
        string? batchNumber = null,
        string? remark = null)
        : base(id)
    {
        OutboundOrderId = outboundOrderId;
        LineNo = lineNo;
        MaterialId = materialId;
        MaterialCode = materialCode;
        MaterialName = materialName;
        RequiredQuantity = requiredQuantity;
        AllocatedQuantity = 0m;
        PickedQuantity = 0m;
        ShippedQuantity = 0m;
        PickingLocationId = null;
        PickingLocationCode = null;
        IssueStrategy = issueStrategy;
        BatchNumber = batchNumber;
        Remark = remark;
    }

    /// <summary>
    /// Set allocated quantity — called during allocation.
    /// </summary>
    public void SetAllocatedQuantity(decimal allocatedQuantity)
    {
        if (allocatedQuantity < 0m)
        {
            throw new BusinessException("WMS:Outbound:InvalidAllocatedQuantity",
                "Allocated quantity must be non-negative.");
        }

        AllocatedQuantity = allocatedQuantity;
    }

    /// <summary>
    /// Set picked quantity — called during picking confirmation.
    /// </summary>
    public void SetPickedQuantity(decimal pickedQuantity)
    {
        if (pickedQuantity < 0m)
        {
            throw new BusinessException("WMS:Outbound:InvalidPickedQuantity",
                "Picked quantity must be non-negative.");
        }

        PickedQuantity = pickedQuantity;
    }

    /// <summary>
    /// Set shipped quantity — called during shipping confirmation.
    /// Validates that shipped quantity does not exceed picked quantity (OB-006).
    /// </summary>
    public void SetShippedQuantity(decimal shippedQuantity)
    {
        if (shippedQuantity < 0m)
        {
            throw new BusinessException("WMS:Outbound:InvalidShippedQuantity",
                "Shipped quantity must be non-negative.");
        }

        if (shippedQuantity > PickedQuantity)
        {
            throw new BusinessException("WMS:Outbound:ShippedExceedsPicked",
                $"Shipped quantity ({shippedQuantity}) exceeds picked quantity ({PickedQuantity}). (OB-006)");
        }

        ShippedQuantity = shippedQuantity;
    }

    /// <summary>
    /// Set picking location — called during allocation or picking.
    /// </summary>
    public void SetPickingLocation(Guid locationId, string locationCode)
    {
        PickingLocationId = locationId;
        PickingLocationCode = locationCode;
    }

    /// <summary>
    /// Set batch number — called during picking or allocation.
    /// </summary>
    public void SetBatchNumber(string batchNumber)
    {
        BatchNumber = batchNumber;
    }
}
