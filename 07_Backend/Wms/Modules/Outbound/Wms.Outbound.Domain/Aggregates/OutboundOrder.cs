using Volo.Abp.Domain.Entities.Auditing;
using Wms.Outbound.Domain.Enums;
using Wms.Outbound.Domain.Events;
using Wms.Shared.Domain.Enums;
using Wms.Shared.Domain.Helpers;

namespace Wms.Outbound.Domain.Aggregates;

/// <summary>
/// OutboundOrder Aggregate Root (AGG-12) — the core aggregate of the Outbound module.
/// Represents an outbound order with associated lines for material requisition, sales, or return.
/// State machine: Draft → Allocated → Picking → Shipped → Completed → Closed (SM-02).
/// Inherits FullAuditedAggregateRoot<Guid> for soft-delete + full audit trail.
/// (ENT-09, Phase 3 DDD Design)
/// </summary>
public class OutboundOrder : FullAuditedAggregateRoot<Guid>
{
    /// <summary>Outbound order number — unique business natural key, auto-generated.</summary>
    public string OutboundOrderNo { get; private set; }

    /// <summary>Outbound type — MaterialRequisition/SalesShipment/ReturnMaterial (Shared Kernel).</summary>
    public OutboundType OutboundType { get; private set; }

    /// <summary>Outbound status — state machine controlled (SM-02).</summary>
    public OutboundStatus OutboundStatus { get; private set; }

    /// <summary>Source warehouse ID.</summary>
    public Guid WarehouseId { get; private set; }

    /// <summary>Source warehouse code — redundant field.</summary>
    public string WarehouseCode { get; private set; }

    /// <summary>Material requisition ID — required when OutboundType = MaterialRequisition.</summary>
    public Guid? MaterialRequisitionId { get; private set; }

    /// <summary>Sales order ID — required when OutboundType = SalesShipment.</summary>
    public Guid? SalesOrderId { get; private set; }

    /// <summary>Return material order ID — required when OutboundType = ReturnMaterial.</summary>
    public Guid? ReturnMaterialOrderId { get; private set; }

    /// <summary>Over-issue ratio — configurable tolerance for excess issue (default 0.00).</summary>
    public decimal OverIssueRatio { get; private set; }

    /// <summary>Whether this is an emergency outbound order.</summary>
    public bool IsEmergency { get; private set; }

    /// <summary>Total required quantity — sum of all line RequiredQuantity.</summary>
    public decimal TotalRequiredQuantity { get; private set; }

    /// <summary>Total allocated quantity — sum of all line AllocatedQuantity.</summary>
    public decimal TotalAllocatedQuantity { get; private set; }

    /// <summary>Total picked quantity — sum of all line PickedQuantity.</summary>
    public decimal TotalPickedQuantity { get; private set; }

    /// <summary>Total shipped quantity — sum of all line ShippedQuantity.</summary>
    public decimal TotalShippedQuantity { get; private set; }

    /// <summary>Whether the order is completed.</summary>
    public bool IsCompleted { get; private set; }

    /// <summary>Completion time.</summary>
    public DateTime? CompletionTime { get; private set; }

    /// <summary>ERP callback status.</summary>
    public ErpCallbackStatus ErpCallbackStatus { get; private set; }

    /// <summary>Remark.</summary>
    public string? Remark { get; private set; }

    /// <summary>Outbound lines — child entities nested within the aggregate.</summary>
    public List<OutboundLine> Lines { get; private set; } = new();

    private OutboundOrder() { }

    public OutboundOrder(
        Guid id,
        OutboundType outboundType,
        Guid warehouseId,
        string warehouseCode,
        decimal overIssueRatio = 0m,
        bool isEmergency = false,
        Guid? materialRequisitionId = null,
        Guid? salesOrderId = null,
        Guid? returnMaterialOrderId = null,
        string? remark = null)
        : base(id)
    {
        OutboundOrderNo = IdGenerator.NewOrderNo("OB");
        OutboundType = outboundType;
        OutboundStatus = OutboundStatus.Draft;
        WarehouseId = warehouseId;
        WarehouseCode = warehouseCode;
        OverIssueRatio = overIssueRatio;
        IsEmergency = isEmergency;
        MaterialRequisitionId = materialRequisitionId;
        SalesOrderId = salesOrderId;
        ReturnMaterialOrderId = returnMaterialOrderId;
        TotalRequiredQuantity = 0m;
        TotalAllocatedQuantity = 0m;
        TotalPickedQuantity = 0m;
        TotalShippedQuantity = 0m;
        IsCompleted = false;
        CompletionTime = null;
        ErpCallbackStatus = ErpCallbackStatus.None;
        Remark = remark;

        // Validate: MaterialRequisition requires MaterialRequisitionId
        if (outboundType == OutboundType.MaterialRequisition && !materialRequisitionId.HasValue)
        {
            throw new BusinessException("WMS:Outbound:MaterialRequisitionRequired",
                "Material requisition ID is required for MaterialRequisition outbound type.");
        }

        if (outboundType == OutboundType.SalesShipment && !salesOrderId.HasValue)
        {
            throw new BusinessException("WMS:Outbound:SalesOrderRequired",
                "Sales order ID is required for SalesShipment outbound type.");
        }

        if (outboundType == OutboundType.ReturnMaterial && !returnMaterialOrderId.HasValue)
        {
            throw new BusinessException("WMS:Outbound:ReturnMaterialOrderRequired",
                "Return material order ID is required for ReturnMaterial outbound type.");
        }

        // Publish creation event (DE-014)
        AddLocalEvent(new OutboundOrderCreatedEvent
        {
            AggregateRootId = Id,
            OrderId = Id,
            OutboundTypeValue = outboundType.Value,
            WarehouseId = warehouseId,
            TotalRequiredQuantity = 0m,
            SourceModule = "Outbound"
        });
    }

    /// <summary>
    /// Add an outbound line. Only allowed in Draft status.
    /// </summary>
    public OutboundLine AddLine(
        Guid lineId,
        int lineNo,
        Guid materialId,
        string materialCode,
        string materialName,
        decimal requiredQuantity,
        int issueStrategyValue = 0,
        string? batchNumber = null,
        string? remark = null)
    {
        if (OutboundStatus != OutboundStatus.Draft)
        {
            throw new BusinessException("WMS:Outbound:StatusNotAllowed",
                $"Cannot add lines when order status is {OutboundStatus.Name}. Only Draft allows this. (OB-001)");
        }

        var strategy = IssueStrategyType.FromValue(issueStrategyValue);
        var line = new OutboundLine(
            lineId, Id, lineNo, materialId, materialCode, materialName,
            requiredQuantity, strategy, batchNumber, remark);

        Lines.Add(line);
        TotalRequiredQuantity = Lines.Sum(l => l.RequiredQuantity);

        return line;
    }

    /// <summary>
    /// Remove an outbound line. Only allowed in Draft status.
    /// </summary>
    public void RemoveLine(Guid lineId)
    {
        if (OutboundStatus != OutboundStatus.Draft)
        {
            throw new BusinessException("WMS:Outbound:StatusNotAllowed",
                $"Cannot remove lines when order status is {OutboundStatus.Name}. (OB-001)");
        }

        var line = Lines.FirstOrDefault(l => l.Id == lineId);
        if (line == null)
        {
            throw new BusinessException("WMS:Outbound:LineNotFound",
                $"Outbound line {lineId} not found in order {OutboundOrderNo}.");
        }

        Lines.Remove(line);
        TotalRequiredQuantity = Lines.Sum(l => l.RequiredQuantity);
    }

    /// <summary>
    /// Allocate inventory — transition from Draft to Allocated (SM-02).
    /// Sets allocated quantities on each line. Validates over-issue ratio.
    /// ⚠️ The caller (DomainService/AppService) is responsible for calling
    /// IInventoryDomainService.ReserveInventoryAsync for each line within the same UoW.
    /// </summary>
    public void Allocate(List<(Guid lineId, decimal allocatedQty, Guid? locationId, string? locationCode)> allocationData)
    {
        if (OutboundStatus != OutboundStatus.Draft)
        {
            throw new BusinessException("WMS:Outbound:StatusNotAllowed",
                $"Cannot allocate when order status is {OutboundStatus.Name}. Only Draft allows allocation. (OB-001)");
        }

        foreach (var (lineId, allocatedQty, locationId, locationCode) in allocationData)
        {
            var line = Lines.FirstOrDefault(l => l.Id == lineId);
            if (line == null)
            {
                throw new BusinessException("WMS:Outbound:LineNotFound",
                    $"Outbound line {lineId} not found.");
            }

            line.SetAllocatedQuantity(allocatedQty);
            if (locationId.HasValue)
            {
                line.SetPickingLocation(locationId.Value, locationCode ?? string.Empty);
            }

            // Check over-issue
            if (allocatedQty > line.RequiredQuantity * (1 + OverIssueRatio))
            {
                AddLocalEvent(new OverIssueDetectedEvent
                {
                    AggregateRootId = Id,
                    OrderId = Id,
                    MaterialId = line.MaterialId,
                    RequiredQuantity = line.RequiredQuantity,
                    ActualQuantity = allocatedQty,
                    SourceModule = "Outbound"
                });

                throw new BusinessException("WMS:Outbound:OverIssueExceeded",
                    $"Allocated quantity ({allocatedQty}) exceeds required quantity ({line.RequiredQuantity}) " +
                    $"by more than allowed over-issue ratio ({OverIssueRatio}). Material: {line.MaterialCode}. (OB-003)");
            }
        }

        TotalAllocatedQuantity = Lines.Sum(l => l.AllocatedQuantity);
        OutboundStatus = OutboundStatus.Allocated;
    }

    /// <summary>
    /// Confirm picking — transition from Allocated to Picking (SM-02).
    /// Sets picked quantities on lines.
    /// </summary>
    public void ConfirmPicking(List<(Guid lineId, decimal pickedQty)> pickingData)
    {
        if (OutboundStatus != OutboundStatus.Allocated)
        {
            throw new BusinessException("WMS:Outbound:StatusNotAllowed",
                $"Cannot confirm picking when order status is {OutboundStatus.Name}. Only Allocated allows picking. (OB-001)");
        }

        foreach (var (lineId, pickedQty) in pickingData)
        {
            var line = Lines.FirstOrDefault(l => l.Id == lineId);
            if (line == null)
            {
                throw new BusinessException("WMS:Outbound:LineNotFound",
                    $"Outbound line {lineId} not found.");
            }

            line.SetPickedQuantity(pickedQty);
        }

        TotalPickedQuantity = Lines.Sum(l => l.PickedQuantity);
        OutboundStatus = OutboundStatus.Picking;
    }

    /// <summary>
    /// Confirm shipping — transition from Picking to Shipped (SM-02).
    /// Validates that shipped quantities match picked quantities (OB-006).
    /// </summary>
    public void ConfirmShipping(List<(Guid lineId, decimal shippedQty)> shippingData)
    {
        if (OutboundStatus != OutboundStatus.Picking)
        {
            throw new BusinessException("WMS:Outbound:StatusNotAllowed",
                $"Cannot confirm shipping when order status is {OutboundStatus.Name}. Only Picking allows shipping. (OB-001)");
        }

        foreach (var (lineId, shippedQty) in shippingData)
        {
            var line = Lines.FirstOrDefault(l => l.Id == lineId);
            if (line == null)
            {
                throw new BusinessException("WMS:Outbound:LineNotFound",
                    $"Outbound line {lineId} not found.");
            }

            line.SetShippedQuantity(shippedQty);
        }

        TotalShippedQuantity = Lines.Sum(l => l.ShippedQuantity);
        OutboundStatus = OutboundStatus.Shipped;

        AddLocalEvent(new OutboundShippedEvent
        {
            AggregateRootId = Id,
            OrderId = Id,
            TotalShippedQuantity = TotalShippedQuantity,
            SourceModule = "Outbound"
        });
    }

    /// <summary>
    /// Complete the outbound order — transition from Shipped to Completed (SM-02).
    /// ⚠️ The caller is responsible for synchronously calling:
    /// - IInventoryDomainService.DecreaseInventoryAsync (actual deduction)
    /// - IInventoryDomainService.ReleaseReservationAsync (release reservation)
    /// within the same UoW transaction (CROSS-002).
    /// </summary>
    public void Complete()
    {
        if (OutboundStatus != OutboundStatus.Shipped)
        {
            throw new BusinessException("WMS:Outbound:StatusNotAllowed",
                $"Cannot complete when order status is {OutboundStatus.Name}. Only Shipped allows completion. (OB-001)");
        }

        OutboundStatus = OutboundStatus.Completed;
        IsCompleted = true;
        CompletionTime = DateTime.UtcNow;

        AddLocalEvent(new OutboundCompletedEvent
        {
            AggregateRootId = Id,
            OrderId = Id,
            OutboundTypeValue = OutboundType.Value,
            TotalQuantity = TotalShippedQuantity,
            SourceModule = "Outbound"
        });
    }

    /// <summary>
    /// Cancel the outbound order — only from Draft status (SM-02).
    /// If in Allocated status, must release allocation first (ReleaseAllocation).
    /// </summary>
    public void Cancel()
    {
        if (OutboundStatus != OutboundStatus.Draft)
        {
            throw new BusinessException("WMS:Outbound:StatusNotAllowed",
                $"Cannot cancel when order status is {OutboundStatus.Name}. Only Draft allows cancellation. (OB-001)");
        }

        OutboundStatus = OutboundStatus.Cancelled;
    }

    /// <summary>
    /// Release allocation — transition from Allocated back to Draft (SM-02).
    /// ⚠️ The caller must synchronously call IInventoryDomainService.ReleaseReservationAsync.
    /// </summary>
    public void ReleaseAllocation()
    {
        if (OutboundStatus != OutboundStatus.Allocated)
        {
            throw new BusinessException("WMS:Outbound:StatusNotAllowed",
                $"Cannot release allocation when order status is {OutboundStatus.Name}. Only Allocated allows release. (OB-001)");
        }

        foreach (var line in Lines)
        {
            line.SetAllocatedQuantity(0m);
            line.SetPickingLocation(Guid.Empty, string.Empty);
        }

        TotalAllocatedQuantity = 0m;
        OutboundStatus = OutboundStatus.Draft;
    }

    /// <summary>
    /// Update remark.
    /// </summary>
    public void SetRemark(string? remark)
    {
        Remark = remark;
    }
}
