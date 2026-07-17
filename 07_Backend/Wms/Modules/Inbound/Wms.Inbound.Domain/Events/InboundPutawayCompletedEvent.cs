using Wms.Shared.Domain.Events;

namespace Wms.Inbound.Domain.Events;

/// <summary>
/// DE-011: InboundPutawayCompletedEvent — raised when putaway is confirmed for an inbound line.
/// Published by InboundOrder aggregate root, subscribed by Inventory and BarcodeLabel modules.
/// </summary>
public class InboundPutawayCompletedEvent : EventDataBase
{
    /// <summary>Inbound order ID.</summary>
    public Guid OrderId { get; set; }

    /// <summary>Material ID put away.</summary>
    public Guid MaterialId { get; set; }

    /// <summary>Location ID where material was put away.</summary>
    public Guid LocationId { get; set; }

    /// <summary>Quantity put away.</summary>
    public decimal Quantity { get; set; }

    /// <summary>Batch number of the putaway material.</summary>
    public string? BatchNo { get; set; }

    /// <summary>Source module — always "Inbound".</summary>
    public string SourceModule { get; set; } = "Inbound";
}
