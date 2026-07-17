using Wms.Shared.Domain.Events;

namespace Wms.Outbound.Domain.Events;

/// <summary>
/// DE-020: OverIssueDetectedEvent — raised when allocated/picked quantity exceeds
/// required quantity * (1 + OverIssueRatio). Subscribed by Notification module.
/// Error code: OB-003.
/// </summary>
public class OverIssueDetectedEvent : EventDataBase
{
    /// <summary>Outbound order ID.</summary>
    public Guid OrderId { get; set; }

    /// <summary>Material ID that exceeded the over-issue ratio.</summary>
    public Guid MaterialId { get; set; }

    /// <summary>Required quantity.</summary>
    public decimal RequiredQuantity { get; set; }

    /// <summary>Actual allocated/picked quantity.</summary>
    public decimal ActualQuantity { get; set; }

    /// <summary>Source module — always "Outbound".</summary>
    public string SourceModule { get; set; } = "Outbound";
}
