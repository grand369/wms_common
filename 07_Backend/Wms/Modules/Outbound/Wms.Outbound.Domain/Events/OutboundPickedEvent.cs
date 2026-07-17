using Wms.Shared.Domain.Events;

namespace Wms.Outbound.Domain.Events;

/// <summary>
/// DE-016: OutboundPickedEvent — raised when picking is confirmed for a line.
/// Published by OutboundLine, no subscriber in v1.0.
/// </summary>
public class OutboundPickedEvent : EventDataBase
{
    /// <summary>Outbound order ID.</summary>
    public Guid OrderId { get; set; }

    /// <summary>Outbound line ID.</summary>
    public Guid LineId { get; set; }

    /// <summary>Material ID picked.</summary>
    public Guid MaterialId { get; set; }

    /// <summary>Picked quantity.</summary>
    public decimal PickedQuantity { get; set; }

    /// <summary>Source module — always "Outbound".</summary>
    public string SourceModule { get; set; } = "Outbound";
}
