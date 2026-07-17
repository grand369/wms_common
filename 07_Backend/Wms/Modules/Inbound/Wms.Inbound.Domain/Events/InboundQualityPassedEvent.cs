using Wms.Shared.Domain.Events;

namespace Wms.Inbound.Domain.Events;

/// <summary>
/// DE-009: InboundQualityPassedEvent — raised when quality inspection passes for an inbound line.
/// Published by InboundOrder aggregate root, subscribed by Inventory and TaskCenter modules.
/// </summary>
public class InboundQualityPassedEvent : EventDataBase
{
    /// <summary>Inbound order ID.</summary>
    public Guid OrderId { get; set; }

    /// <summary>Inbound line ID.</summary>
    public Guid LineId { get; set; }

    /// <summary>Material ID that passed quality inspection.</summary>
    public Guid MaterialId { get; set; }

    /// <summary>Received quantity that passed inspection.</summary>
    public decimal Quantity { get; set; }

    /// <summary>Batch number of the inspected material.</summary>
    public string? BatchNo { get; set; }

    /// <summary>Source module — always "Inbound".</summary>
    public string SourceModule { get; set; } = "Inbound";
}
