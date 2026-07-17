using System;

namespace Wms.Transfer.Domain.Aggregates;

/// <summary>
/// TransferLine — sub-entity of TransferOrder (AGG-15).
/// Tracks per-line outbound/inbound confirmed quantities.
/// </summary>
public class TransferLine : FullAuditedEntity<Guid>
{
    public Guid TransferOrderId { get; private set; }
    public int LineNo { get; private set; }
    public Guid MaterialId { get; private set; }
    public string MaterialCode { get; private set; }
    public decimal TransferQuantity { get; private set; }
    public decimal OutboundConfirmedQuantity { get; private set; }
    public decimal InboundConfirmedQuantity { get; private set; }

    protected TransferLine() { } // EF

    public TransferLine(
        Guid id,
        Guid transferOrderId,
        int lineNo,
        Guid materialId,
        string materialCode,
        decimal transferQuantity)
    {
        Id = id;
        TransferOrderId = transferOrderId;
        LineNo = lineNo;
        MaterialId = materialId;
        MaterialCode = materialCode ?? throw new ArgumentNullException(nameof(materialCode));
        TransferQuantity = transferQuantity;
        OutboundConfirmedQuantity = 0;
        InboundConfirmedQuantity = 0;
    }

    internal void SetOutboundConfirmedQuantity(decimal qty)
    {
        if (qty > TransferQuantity)
            throw new BusinessException("Wms.Transfer:0301", "Outbound confirmed quantity exceeds transfer quantity.");
        OutboundConfirmedQuantity = qty;
    }

    internal void SetInboundConfirmedQuantity(decimal qty)
    {
        if (qty > OutboundConfirmedQuantity)
            throw new BusinessException("Wms.Transfer:0302", "Inbound confirmed quantity exceeds outbound confirmed quantity.");
        InboundConfirmedQuantity = qty;
    }
}
