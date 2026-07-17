using System;
using System.Collections.Generic;
using Wms.Production.Domain.Enums;

namespace Wms.Production.Domain.Aggregates;

/// <summary>
/// MaterialRequisition Aggregate Root — AGG-19
/// Auto-generated from production order + BOM (REQ-PD-001).
/// Tracks requisition lines with required/issued quantities.
/// </summary>
public class MaterialRequisition : FullAuditedAggregateRoot<Guid>
{
    public string RequisitionNo { get; private set; }
    public Guid ProductionOrderId { get; private set; }
    public string ProductionOrderNo { get; private set; }
    public RequisitionStatus RequisitionStatus { get; private set; }
    public Guid WarehouseId { get; private set; }
    public string WarehouseCode { get; private set; }

    public List<MaterialRequisitionLine> Lines { get; private set; } = new();

    protected MaterialRequisition() { }

    public MaterialRequisition(
        Guid id, string requisitionNo, Guid productionOrderId, string productionOrderNo,
        Guid warehouseId, string warehouseCode)
    {
        Id = id;
        RequisitionNo = requisitionNo ?? throw new ArgumentNullException(nameof(requisitionNo));
        ProductionOrderId = productionOrderId;
        ProductionOrderNo = productionOrderNo ?? throw new ArgumentNullException(nameof(productionOrderNo));
        RequisitionStatus = RequisitionStatus.Draft;
        WarehouseId = warehouseId;
        WarehouseCode = warehouseCode ?? throw new ArgumentNullException(nameof(warehouseCode));
    }

    public void Submit() { if (RequisitionStatus != RequisitionStatus.Draft) throw new BusinessException("Wms.Production:0101"); RequisitionStatus = RequisitionStatus.Submitted; }
    public void Complete() { if (RequisitionStatus != RequisitionStatus.Issued && RequisitionStatus != RequisitionStatus.PartiallyIssued) throw new BusinessException("Wms.Production:0102"); RequisitionStatus = RequisitionStatus.Completed; }
    public void Cancel() { if (RequisitionStatus != RequisitionStatus.Draft) throw new BusinessException("Wms.Production:0103"); RequisitionStatus = RequisitionStatus.Cancelled; }

    public MaterialRequisitionLine AddLine(int lineNo, Guid materialId, string materialCode, decimal requiredQuantity)
    {
        var line = new MaterialRequisitionLine(Guid.NewGuid(), Id, lineNo, materialId, materialCode, requiredQuantity);
        Lines.Add(line);
        return line;
    }

    public void IssueLine(int lineNo, decimal issuedQty)
    {
        var line = Lines.Find(l => l.LineNo == lineNo);
        if (line == null) throw new BusinessException("Wms.Production:0201");
        line.Issue(issuedQty);
        // BR-023: Over-issuance check
        if (issuedQty > line.RequiredQuantity * 1.1m) throw new BusinessException("Wms.Production:0202", "Over-issuance exceeds 10% tolerance. Approval required.");
    }
}
