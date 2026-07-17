using Wms.Shared.Domain.Events;

namespace Wms.Inbound.Domain.Events;

/// <summary>
/// DE-010: InboundQualityFailedEvent — raised when quality inspection fails for an inbound line.
/// Published by InboundOrder aggregate root, subscribed by Notification module.
/// </summary>
public class InboundQualityFailedEvent : EventDataBase
{
    /// <summary>Inbound order ID.</summary>
    public Guid OrderId { get; set; }

    /// <summary>Inbound line ID.</summary>
    public Guid LineId { get; set; }

    /// <summary>Material ID that failed quality inspection.</summary>
    public Guid MaterialId { get; set; }

    /// <summary>Received quantity that failed inspection.</summary>
    public decimal Quantity { get; set; }

    /// <summary>Source module — always "Inbound".</summary>
    public string SourceModule { get; set; } = "Inbound";
}
