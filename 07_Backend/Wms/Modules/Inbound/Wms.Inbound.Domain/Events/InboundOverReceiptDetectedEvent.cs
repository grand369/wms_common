using Wms.Shared.Domain.Events;

namespace Wms.Inbound.Domain.Events;

/// <summary>
/// DE-013: InboundOverReceiptDetectedEvent — raised when received quantity exceeds
/// plan quantity * (1 + OverReceiptRatio). Subscribed by Notification module.
/// Error code: IN-002.
/// </summary>
public class InboundOverReceiptDetectedEvent : EventDataBase
{
    /// <summary>Inbound order ID.</summary>
    public Guid OrderId { get; set; }

    /// <summary>Material ID that exceeded the over-receipt ratio.</summary>
    public Guid MaterialId { get; set; }

    /// <summary>Planned quantity.</summary>
    public decimal PlanQuantity { get; set; }

    /// <summary>Received quantity.</summary>
    public decimal ReceivedQuantity { get; set; }

    /// <summary>Over-receipt ratio that was exceeded.</summary>
    public decimal Ratio { get; set; }

    /// <summary>Source module — always "Inbound".</summary>
    public string SourceModule { get; set; } = "Inbound";
}
