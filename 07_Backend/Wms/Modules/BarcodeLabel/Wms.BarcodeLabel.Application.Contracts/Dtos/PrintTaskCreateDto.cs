using System;

namespace Wms.BarcodeLabel.Application.Contracts.Dtos;

public class PrintTaskCreateDto
{
    public Guid TemplateId { get; set; }
    public string SourceOrderType { get; set; } = string.Empty;
    public Guid SourceOrderId { get; set; }
    public string PrintContent { get; set; } = string.Empty;
    public int PrintQuantity { get; set; }
    public string? PrinterId { get; set; }
    public string? PrinterName { get; set; }
}
