using Wms.Shared.Domain.Events;

namespace Wms.BarcodeLabel.Domain.Events;

/// <summary>
/// DE-034: PrintFailedEvent — raised when a print task fails.
/// </summary>
public class PrintFailedEvent : EventDataBase
{
    /// <summary>Print task ID.</summary>
    public Guid PrintTaskId { get; set; }

    /// <summary>Print task number.</summary>
    public string TaskNo { get; set; } = string.Empty;

    /// <summary>Error message describing the failure.</summary>
    public string? ErrorMessage { get; set; }
}
