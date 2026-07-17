using Wms.Shared.Domain.Events;

namespace Wms.BarcodeLabel.Domain.Events;

/// <summary>
/// DE-034: PrintCompletedEvent — raised when a print task completes successfully.
/// </summary>
public class PrintCompletedEvent : EventDataBase
{
    /// <summary>Print task ID.</summary>
    public Guid PrintTaskId { get; set; }

    /// <summary>Print task number.</summary>
    public string TaskNo { get; set; } = string.Empty;

    /// <summary>Printer ID that completed the job.</summary>
    public string? PrinterId { get; set; }
}
