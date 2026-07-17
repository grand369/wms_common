using Wms.Shared.Domain.Events;

namespace Wms.Outbound.Domain.Events;

/// <summary>
/// DE-014: OutboundOrderCreatedEvent — raised when an outbound order is created.
/// Published by OutboundOrder aggregate root, subscribed by TaskCenter module.
/// </summary>
public class OutboundOrderCreatedEvent : EventDataBase
{
    /// <summary>Outbound order ID.</summary>
    public Guid OrderId { get; set; }

    /// <summary>Outbound type value (MaterialRequisition=1/SalesShipment=2/ReturnMaterial=3).</summary>
    public int OutboundTypeValue { get; set; }

    /// <summary>Source warehouse ID.</summary>
    public Guid WarehouseId { get; set; }

    /// <summary>Total required quantity.</summary>
    public decimal TotalRequiredQuantity { get; set; }

    /// <summary>Source module — always "Outbound".</summary>
    public string SourceModule { get; set; } = "Outbound";
}
