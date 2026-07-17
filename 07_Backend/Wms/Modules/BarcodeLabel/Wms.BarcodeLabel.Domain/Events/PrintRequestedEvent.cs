using Wms.Shared.Domain.Events;

namespace Wms.BarcodeLabel.Domain.Events;

/// <summary>
/// DE-034: PrintRequestedEvent — raised when a print is requested from any source module.
/// BarcodeLabel module subscribes to auto-create PrintTask.
/// </summary>
public class PrintRequestedEvent : EventDataBase
{
    /// <summary>Source order ID that triggered the print request.</summary>
    public Guid SourceOrderId { get; set; }

    /// <summary>Source order type (e.g., "Inbound", "Outbound").</summary>
    public string SourceOrderType { get; set; } = string.Empty;

    /// <summary>Barcode type value.</summary>
    public int BarcodeTypeValue { get; set; }

    /// <summary>Material ID that the barcode references (optional).</summary>
    public Guid? MaterialId { get; set; }

    /// <summary>Print type / category.</summary>
    public string? PrintType { get; set; }
}
