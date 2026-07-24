using Volo.Abp.Domain.Entities.Auditing;
using Wms.Inbound.Domain.Enums;
using Wms.Inbound.Domain.Events;
using Wms.Shared.Domain.Enums;
using Wms.Shared.Domain.Helpers;

namespace Wms.Inbound.Domain.Aggregates;

/// <summary>
/// InboundOrder Aggregate Root (AGG-10) — the core aggregate of the Inbound module.
/// Represents an inbound receipt order with associated lines.
/// State machine: Draft → Confirmed → Inspecting/Putaway → Completed → Closed (SM-01).
/// Inherits FullAuditedAggregateRoot<Guid> for soft-delete + full audit trail.
/// (ENT-08, Phase 3 DDD Design)
/// </summary>
public class InboundOrder : FullAuditedAggregateRoot<Guid>
{
    /// <summary>Inbound order number — unique business natural key, auto-generated.</summary>
    public string InboundOrderNo { get; private set; }

    /// <summary>Inbound type — PurchaseReceipt/ProductionReceipt/ReturnReceipt (Shared Kernel).</summary>
    public InboundType InboundType { get; private set; }

    /// <summary>Inbound status — state machine controlled (SM-01).</summary>
    public InboundStatus InboundStatus { get; private set; }

    /// <summary>Target warehouse ID.</summary>
    public Guid WarehouseId { get; private set; }

    /// <summary>Target warehouse code — redundant field for query optimization.</summary>
    public string WarehouseCode { get; private set; }

    /// <summary>Purchase order ID — required when InboundType = PurchaseReceipt.</summary>
    public Guid? PurchaseOrderId { get; private set; }

    /// <summary>Purchase order number — redundant.</summary>
    public string? PurchaseOrderNo { get; private set; }

    /// <summary>Production order ID — required when InboundType = ProductionReceipt.</summary>
    public Guid? ProductionOrderId { get; private set; }

    /// <summary>Return order ID — required when InboundType = ReturnReceipt.</summary>
    public Guid? ReturnOrderId { get; private set; }

    /// <summary>Supplier ID — required when InboundType = PurchaseReceipt.</summary>
    public Guid? SupplierId { get; private set; }

    /// <summary>Supplier name — redundant field.</summary>
    public string? SupplierName { get; private set; }

    /// <summary>Over-receipt ratio — configurable tolerance for excess receipt (default 0.00).</summary>
    public decimal OverReceiptRatio { get; private set; }

    /// <summary>Whether quality inspection is required (default true).</summary>
    public bool QualityInspectionRequired { get; private set; }

    /// <summary>Total planned quantity — sum of all line PlanQuantity.</summary>
    public decimal TotalPlanQuantity { get; private set; }

    /// <summary>Total received quantity — sum of all line ReceivedQuantity.</summary>
    public decimal TotalReceivedQuantity { get; private set; }

    /// <summary>Whether the order is completed.</summary>
    public bool IsCompleted { get; private set; }

    /// <summary>Completion time — set when order transitions to Completed.</summary>
    public DateTime? CompletionTime { get; private set; }

    /// <summary>ERP callback status — None/Success/Failed/Pending.</summary>
    public ErpCallbackStatus ErpCallbackStatus { get; private set; }

    /// <summary>Remark — optional note.</summary>
    public string? Remark { get; private set; }

    /// <summary>Inbound lines — child entities nested within the aggregate.</summary>
    public List<InboundLine> Lines { get; private set; } = new();

    private InboundOrder() { }

    public InboundOrder(
        Guid id,
        InboundType inboundType,
        Guid warehouseId,
        string warehouseCode,
        decimal overReceiptRatio = 0m,
        bool qualityInspectionRequired = true,
        Guid? purchaseOrderId = null,
        string? purchaseOrderNo = null,
        Guid? productionOrderId = null,
        Guid? returnOrderId = null,
        Guid? supplierId = null,
        string? supplierName = null,
        string? remark = null)
        : base(id)
    {
        InboundOrderNo = IdGenerator.NewOrderNo("IN");
        InboundType = inboundType;
        InboundStatus = InboundStatus.Draft;
        WarehouseId = warehouseId;
        WarehouseCode = warehouseCode;
        OverReceiptRatio = overReceiptRatio;
        QualityInspectionRequired = qualityInspectionRequired;
        PurchaseOrderId = purchaseOrderId;
        PurchaseOrderNo = purchaseOrderNo;
        ProductionOrderId = productionOrderId;
        ReturnOrderId = returnOrderId;
        SupplierId = supplierId;
        SupplierName = supplierName;
        TotalPlanQuantity = 0m;
        TotalReceivedQuantity = 0m;
        IsCompleted = false;
        CompletionTime = null;
        ErpCallbackStatus = ErpCallbackStatus.None;
        Remark = remark;

        // Validate: PurchaseReceipt requires PurchaseOrderId + SupplierId
        //if (inboundType == InboundType.PurchaseReceipt && !purchaseOrderId.HasValue)
        //{
        //    throw new BusinessException("WMS:Inbound:PurchaseOrderRequired",
        //        "Purchase order ID is required for PurchaseReceipt inbound type.");
        //}

        //if (inboundType == InboundType.ProductionReceipt && !productionOrderId.HasValue)
        //{
        //    throw new BusinessException("WMS:Inbound:ProductionOrderRequired",
        //        "Production order ID is required for ProductionReceipt inbound type.");
        //}

        //if (inboundType == InboundType.ReturnReceipt && !returnOrderId.HasValue)
        //{
        //    throw new BusinessException("WMS:Inbound:ReturnOrderRequired",
        //        "Return order ID is required for ReturnReceipt inbound type.");
        //}

        // Publish creation event (DE-008)
        AddLocalEvent(new InboundOrderCreatedEvent
        {
            AggregateRootId = Id,
            OrderId = Id,
            InboundTypeValue = inboundType.Value,
            WarehouseId = warehouseId,
            TotalPlanQuantity = 0m,
            SourceModule = "Inbound"
        });
    }

    /// <summary>
    /// Add an inbound line to the order. Only allowed in Draft status (IN-001).
    /// Recalculates TotalPlanQuantity.
    /// </summary>
    public InboundLine AddLine(
        Guid lineId,
        int lineNo,
        Guid materialId,
        string materialCode,
        string materialName,
        string unit,
        decimal planQuantity,
        string? batchNumber = null,
        DateTime? expiryDate = null,
        DateTime? productionDate = null,
        string? remark = null)
    {
        if (InboundStatus != InboundStatus.Draft)
        {
            throw new BusinessException("WMS:Inbound:StatusNotAllowed",
                $"Cannot add lines when order status is {InboundStatus.Name}. Only Draft status allows this operation. (IN-001)");
        }

        var line = new InboundLine(
            lineId, Id, lineNo, materialId, materialCode, materialName, unit,
            planQuantity, batchNumber, expiryDate, productionDate, remark);

        Lines.Add(line);
        TotalPlanQuantity = Lines.Sum(l => l.PlanQuantity);

        return line;
    }

    /// <summary>
    /// Remove an inbound line from the order. Only allowed in Draft status.
    /// </summary>
    public void RemoveLine(Guid lineId)
    {
        if (InboundStatus != InboundStatus.Draft)
        {
            throw new BusinessException("WMS:Inbound:StatusNotAllowed",
                $"Cannot remove lines when order status is {InboundStatus.Name}. Only Draft status allows this operation. (IN-001)");
        }

        var line = Lines.FirstOrDefault(l => l.Id == lineId);
        if (line == null)
        {
            throw new BusinessException("WMS:Inbound:LineNotFound",
                $"Inbound line {lineId} not found in order {InboundOrderNo}.");
        }

        Lines.Remove(line);
        TotalPlanQuantity = Lines.Sum(l => l.PlanQuantity);
    }

    /// <summary>
    /// Confirm receipt — transition from Draft to Confirmed (SM-01).
    /// Validates that received quantities are recorded and checks over-receipt ratio.
    /// </summary>
    public void ConfirmReceipt()
    {
        if (InboundStatus != InboundStatus.Draft)
        {
            throw new BusinessException("WMS:Inbound:StatusNotAllowed",
                $"Cannot confirm receipt when order status is {InboundStatus.Name}. Only Draft status allows confirmation. (IN-001)");
        }

        if (Lines.Count == 0)
        {
            throw new BusinessException("WMS:Inbound:NoLines",
                "Cannot confirm an inbound order with no lines.");
        }

        // Validate over-receipt for each line
        foreach (var line in Lines)
        {
            if (line.ReceivedQuantity > line.PlanQuantity * (1 + OverReceiptRatio))
            {
                AddLocalEvent(new InboundOverReceiptDetectedEvent
                {
                    AggregateRootId = Id,
                    OrderId = Id,
                    MaterialId = line.MaterialId,
                    PlanQuantity = line.PlanQuantity,
                    ReceivedQuantity = line.ReceivedQuantity,
                    Ratio = OverReceiptRatio,
                    SourceModule = "Inbound"
                });

                throw new BusinessException("WMS:Inbound:OverReceiptExceeded",
                    $"Received quantity ({line.ReceivedQuantity}) exceeds plan quantity ({line.PlanQuantity}) " +
                    $"by more than the allowed over-receipt ratio ({OverReceiptRatio}). Material: {line.MaterialCode}. (IN-002)");
            }
        }

        TotalReceivedQuantity = Lines.Sum(l => l.ReceivedQuantity);
        InboundStatus = InboundStatus.Confirmed;
    }

    /// <summary>
    /// Start quality inspection — transition from Confirmed to Inspecting (SM-01).
    /// Only valid when QualityInspectionRequired = true.
    /// </summary>
    public void StartQualityInspection()
    {
        if (InboundStatus != InboundStatus.Confirmed)
        {
            throw new BusinessException("WMS:Inbound:StatusNotAllowed",
                $"Cannot start quality inspection when order status is {InboundStatus.Name}. Only Confirmed status allows this. (IN-001)");
        }

        if (!QualityInspectionRequired)
        {
            // Skip quality inspection → go directly to Putaway
            InboundStatus = InboundStatus.Putaway;
            foreach (var line in Lines)
            {
                line.SetQualityStatus(QualityStatus.Skip);
            }
            return;
        }

        InboundStatus = InboundStatus.Inspecting;
    }

    /// <summary>
    /// Quality pass — transition Inspecting line to Qualified (DE-009).
    /// If all lines pass, transition order to Putaway.
    /// </summary>
    public void QualityPass(Guid lineId)
    {
        if (InboundStatus != InboundStatus.Inspecting)
        {
            throw new BusinessException("WMS:Inbound:StatusNotAllowed",
                $"Cannot process quality pass when order status is {InboundStatus.Name}. Only Inspecting status allows this. (IN-001)");
        }

        var line = Lines.FirstOrDefault(l => l.Id == lineId);
        if (line == null)
        {
            throw new BusinessException("WMS:Inbound:LineNotFound",
                $"Inbound line {lineId} not found.");
        }

        line.SetQualityStatus(QualityStatus.Qualified);

        AddLocalEvent(new InboundQualityPassedEvent
        {
            AggregateRootId = Id,
            OrderId = Id,
            LineId = lineId,
            MaterialId = line.MaterialId,
            Quantity = line.ReceivedQuantity,
            BatchNo = line.BatchNumber,
            SourceModule = "Inbound"
        });

        // If all lines have quality result, transition order
        if (Lines.All(l => l.QualityStatus == QualityStatus.Qualified || l.QualityStatus == QualityStatus.Skip))
        {
            InboundStatus = InboundStatus.Putaway;
        }
    }

    /// <summary>
    /// Quality fail — transition line to Unqualified (DE-010), order to Isolated.
    /// </summary>
    public void QualityFail(Guid lineId)
    {
        if (InboundStatus != InboundStatus.Inspecting)
        {
            throw new BusinessException("WMS:Inbound:StatusNotAllowed",
                $"Cannot process quality fail when order status is {InboundStatus.Name}. Only Inspecting status allows this. (IN-001)");
        }

        var line = Lines.FirstOrDefault(l => l.Id == lineId);
        if (line == null)
        {
            throw new BusinessException("WMS:Inbound:LineNotFound",
                $"Inbound line {lineId} not found.");
        }

        line.SetQualityStatus(QualityStatus.Unqualified);

        AddLocalEvent(new InboundQualityFailedEvent
        {
            AggregateRootId = Id,
            OrderId = Id,
            LineId = lineId,
            MaterialId = line.MaterialId,
            Quantity = line.ReceivedQuantity,
            SourceModule = "Inbound"
        });

        // If any line fails, transition order to Isolated
        InboundStatus = InboundStatus.Isolated;
    }

    /// <summary>
    /// Confirm putaway for a specific line — sets putaway location (DE-011).
    /// When all lines have putaway locations confirmed, order can transition to Completed.
    /// </summary>
    public void ConfirmPutaway(Guid lineId, Guid warehouseId, string warehouseCode, Guid areaId, string areaCode, Guid locationId, string locationCode, decimal putawayQty)
    {
        if (InboundStatus != InboundStatus.Putaway)
        {
            throw new BusinessException("WMS:Inbound:StatusNotAllowed",
                $"Cannot confirm putaway when order status is {InboundStatus.Name}. Only Putaway status allows this. (IN-001)");
        }

        var line = Lines.FirstOrDefault(l => l.Id == lineId);
        if (line == null)
        {
            throw new BusinessException("WMS:Inbound:LineNotFound",
                $"Inbound line {lineId} not found.");
        }

        // Quality check — unqualified lines cannot be put away (IN-005)
        if (line.QualityStatus == QualityStatus.Unqualified)
        {
            throw new BusinessException("WMS:Inbound:UnqualifiedPutaway",
                $"Cannot put away unqualified material {line.MaterialCode}. Quality status: {line.QualityStatus.Name}. (IN-005)");
        }

        line.SetPutawayLocation(warehouseId, warehouseCode, areaId, areaCode, locationId, locationCode);

        AddLocalEvent(new InboundPutawayCompletedEvent
        {
            AggregateRootId = Id,
            OrderId = Id,
            MaterialId = line.MaterialId,
            LocationId = locationId,
            Quantity = putawayQty,
            BatchNo = line.BatchNumber,
            SourceModule = "Inbound"
        });
    }

    /// <summary>
    /// Complete the inbound order — transition from Putaway to Completed (SM-01).
    /// Requires all lines to have putaway locations confirmed.
    /// Publishes InboundCompletedEvent (DE-012).
    /// </summary>
    public void Complete()
    {
        if (InboundStatus != InboundStatus.Putaway)
        {
            throw new BusinessException("WMS:Inbound:StatusNotAllowed",
                $"Cannot complete when order status is {InboundStatus.Name}. Only Putaway status allows completion. (IN-001)");
        }

        // Validate all lines have putaway location
        foreach (var line in Lines)
        {
            if (!line.PutawayLocationId.HasValue)
            {
                throw new BusinessException("WMS:Inbound:PutawayNotConfirmed",
                    $"Line {line.LineNo} (Material: {line.MaterialCode}) has not been put away yet.");
            }
        }

        InboundStatus = InboundStatus.Completed;
        IsCompleted = true;
        CompletionTime = DateTime.UtcNow;

        AddLocalEvent(new InboundCompletedEvent
        {
            AggregateRootId = Id,
            OrderId = Id,
            InboundTypeValue = InboundType.Value,
            TotalQuantity = TotalReceivedQuantity,
            SourceModule = "Inbound"
        });
    }

    /// <summary>
    /// Cancel the inbound order — can only be done in Draft or Confirmed status (SM-01).
    /// Confirmed status requires approval (placeholder in v1.0).
    /// </summary>
    public void Cancel()
    {
        if (InboundStatus != InboundStatus.Draft && InboundStatus != InboundStatus.Confirmed)
        {
            throw new BusinessException("WMS:Inbound:StatusNotAllowed",
                $"Cannot cancel when order status is {InboundStatus.Name}. Only Draft or Confirmed status allows cancellation. (IN-001)");
        }

        InboundStatus = InboundStatus.Cancelled;
    }

    /// <summary>
    /// Update received quantity for a specific line — called during receipt confirmation.
    /// Validates over-receipt ratio.
    /// </summary>
    public void ReceiveLineQuantity(Guid lineId, decimal receivedQuantity, string? batchNumber = null)
    {
        var line = Lines.FirstOrDefault(l => l.Id == lineId);
        if (line == null)
        {
            throw new BusinessException("WMS:Inbound:LineNotFound",
                $"Inbound line {lineId} not found.");
        }

        line.ReceiveQuantity(receivedQuantity);
        if (batchNumber != null)
        {
            line.SetBatchNumber(batchNumber);
        }

        // Check over-receipt for this line
        if (receivedQuantity > line.PlanQuantity * (1 + OverReceiptRatio))
        {
            AddLocalEvent(new InboundOverReceiptDetectedEvent
            {
                AggregateRootId = Id,
                OrderId = Id,
                MaterialId = line.MaterialId,
                PlanQuantity = line.PlanQuantity,
                ReceivedQuantity = receivedQuantity,
                Ratio = OverReceiptRatio,
                SourceModule = "Inbound"
            });
        }

        TotalReceivedQuantity = Lines.Sum(l => l.ReceivedQuantity);
    }

    /// <summary>
    /// Update remark.
    /// </summary>
    public void SetRemark(string? remark)
    {
        Remark = remark;
    }

    /// <summary>
    /// Update supplier information. Only allowed in Draft status.
    /// </summary>
    public void SetSupplier(Guid? supplierId, string? supplierName)
    {
        if (InboundStatus != InboundStatus.Draft)
        {
            throw new BusinessException("WMS:Inbound:StatusNotAllowed",
                $"Cannot update supplier when order status is {InboundStatus.Name}. Only Draft status allows this operation. (IN-001)");
        }

        SupplierId = supplierId;
        SupplierName = supplierName;
    }

    /// <summary>
    /// Update purchase order information. Only allowed in Draft status.
    /// </summary>
    public void SetPurchaseOrder(Guid? purchaseOrderId, string? purchaseOrderNo)
    {
        if (InboundStatus != InboundStatus.Draft)
        {
            throw new BusinessException("WMS:Inbound:StatusNotAllowed",
                $"Cannot update purchase order when order status is {InboundStatus.Name}. Only Draft status allows this operation. (IN-001)");
        }

        PurchaseOrderId = purchaseOrderId;
        PurchaseOrderNo = purchaseOrderNo;
    }

    /// <summary>
    /// Update order details - replace all lines with new lines. Only allowed in Draft status.
    /// </summary>
    public void UpdateLines(List<InboundLine> newLines)
    {
        if (InboundStatus != InboundStatus.Draft)
        {
            throw new BusinessException("WMS:Inbound:StatusNotAllowed",
                $"Cannot update lines when order status is {InboundStatus.Name}. Only Draft status allows this operation. (IN-001)");
        }

        Lines.Clear();
        Lines.AddRange(newLines);
        TotalPlanQuantity = Lines.Sum(l => l.PlanQuantity);
    }
}
