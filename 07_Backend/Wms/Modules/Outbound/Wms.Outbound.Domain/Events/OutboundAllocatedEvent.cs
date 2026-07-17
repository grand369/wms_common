using Wms.Shared.Domain.Events;

namespace Wms.Outbound.Domain.Events;

/// <summary>
/// DE-015: OutboundAllocatedEvent — raised when inventory allocation is completed for a line.
/// Published by OutboundOrder aggregate root, subscribed by TaskCenter module.
/// </summary>
public class OutboundAllocatedEvent : EventDataBase
{
    /// <summary>Outbound order ID.</summary>
    public Guid OrderId { get; set; }

    /// <summary>Outbound line ID.</summary>
    public Guid LineId { get; set; }

    /// <summary>Material ID allocated.</summary>
    public Guid MaterialId { get; set; }

    /// <summary>Allocated quantity.</summary>
    public decimal AllocatedQuantity { get; set; }

    /// <summary>Picking location ID.</summary>
    public Guid? LocationId { get; set; }

    /// <summary>Source module — always "Outbound".</summary>
    public string SourceModule { get; set; } = "Outbound";
}
