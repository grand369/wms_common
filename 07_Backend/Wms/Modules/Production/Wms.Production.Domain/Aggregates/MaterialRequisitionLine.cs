using System;

namespace Wms.Production.Domain.Aggregates;

/// <summary>MaterialRequisitionLine — sub-entity of AGG-19</summary>
public class MaterialRequisitionLine : FullAuditedEntity<Guid>
{
    public Guid RequisitionId { get; private set; }
    public int LineNo { get; private set; }
    public Guid MaterialId { get; private set; }
    public string MaterialCode { get; private set; }
    public decimal RequiredQuantity { get; private set; }
    public decimal IssuedQuantity { get; private set; }

    protected MaterialRequisitionLine() { }

    public MaterialRequisitionLine(Guid id, Guid requisitionId, int lineNo, Guid materialId, string materialCode, decimal requiredQuantity)
    {
        Id = id; RequisitionId = requisitionId; LineNo = lineNo;
        MaterialId = materialId; MaterialCode = materialCode ?? throw new ArgumentNullException(nameof(materialCode));
        RequiredQuantity = requiredQuantity; IssuedQuantity = 0;
    }

    internal void Issue(decimal qty) { IssuedQuantity += qty; }
}
