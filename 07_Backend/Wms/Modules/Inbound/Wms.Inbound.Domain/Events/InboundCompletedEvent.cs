using Wms.Shared.Domain.Events;

namespace Wms.Inbound.Domain.Events;

/// <summary>
/// DE-012: InboundCompletedEvent — the most important inbound event.
/// Raised when inbound order is fully completed. Inventory module subscribes
/// to increase inventory synchronously (CROSS-002).
/// ⚠️ In v1.0, inventory increase is done via DI call, not async event.
/// This event is published for Notification/ERP callback purposes.
/// </summary>
public class InboundCompletedEvent : EventDataBase
{
    /// <summary>Inbound order ID.</summary>
    public Guid OrderId { get; set; }

    /// <summary>Inbound type value.</summary>
    public int InboundTypeValue { get; set; }

    /// <summary>Total quantity received.</summary>
    public decimal TotalQuantity { get; set; }

    /// <summary>Source module — always "Inbound".</summary>
    public string SourceModule { get; set; } = "Inbound";
}
