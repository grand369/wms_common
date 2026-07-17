using Wms.Shared.Domain.Events;

namespace Wms.Outbound.Domain.Events;

/// <summary>
/// DE-018: OutboundCompletedEvent — the most important outbound event.
/// Raised when outbound order is fully completed. Inventory module subscribes
/// to decrease inventory synchronously (CROSS-002).
/// ⚠️ In v1.0, inventory decrease is done via DI call, not async event.
/// This event is published for Notification/ERP callback purposes.
/// </summary>
public class OutboundCompletedEvent : EventDataBase
{
    /// <summary>Outbound order ID.</summary>
    public Guid OrderId { get; set; }

    /// <summary>Outbound type value.</summary>
    public int OutboundTypeValue { get; set; }

    /// <summary>Total quantity shipped.</summary>
    public decimal TotalQuantity { get; set; }

    /// <summary>Source module — always "Outbound".</summary>
    public string SourceModule { get; set; } = "Outbound";
}
