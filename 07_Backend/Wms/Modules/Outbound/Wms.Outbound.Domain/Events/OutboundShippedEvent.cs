using Wms.Shared.Domain.Events;

namespace Wms.Outbound.Domain.Events;

/// <summary>
/// DE-017: OutboundShippedEvent — raised when shipping is confirmed for an outbound order.
/// Published by OutboundOrder, subscribed by Inventory and ERP modules.
/// </summary>
public class OutboundShippedEvent : EventDataBase
{
    /// <summary>Outbound order ID.</summary>
    public Guid OrderId { get; set; }

    /// <summary>Total shipped quantity.</summary>
    public decimal TotalShippedQuantity { get; set; }

    /// <summary>Source module — always "Outbound".</summary>
    public string SourceModule { get; set; } = "Outbound";
}
