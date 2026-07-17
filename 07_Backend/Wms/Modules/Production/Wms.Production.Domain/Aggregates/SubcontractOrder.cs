using System;
using Wms.Production.Domain.Enums;

namespace Wms.Production.Domain.Aggregates;

/// <summary>
/// SubcontractOrder Aggregate Root — AGG-20 (v2.0 placeholder)
/// Tracks subcontract processing: send materials → receive finished goods + scrap
/// </summary>
public class SubcontractOrder : FullAuditedAggregateRoot<Guid>
{
    public string SubcontractOrderNo { get; private set; }
    public Guid VendorId { get; private set; }
    public string VendorName { get; private set; }
    public int SubcontractStatusValue { get; private set; }
    public decimal SentQuantity { get; private set; }
    public decimal ReceivedQuantity { get; private set; }
    public decimal LossRate { get; private set; }

    protected SubcontractOrder() { }

    public SubcontractOrder(Guid id, string orderNo, Guid vendorId, string vendorName, decimal sentQuantity, decimal lossRate = 0)
    {
        Id = id; SubcontractOrderNo = orderNo ?? throw new ArgumentNullException(nameof(orderNo));
        VendorId = vendorId; VendorName = vendorName ?? throw new ArgumentNullException(nameof(vendorName));
        SentQuantity = sentQuantity; ReceivedQuantity = 0; LossRate = lossRate;
        SubcontractStatusValue = 0; // Draft
    }

    public void Receive(decimal receivedQty) { ReceivedQuantity += receivedQty; }
}
