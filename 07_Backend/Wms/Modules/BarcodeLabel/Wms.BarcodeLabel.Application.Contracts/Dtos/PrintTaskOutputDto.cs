using System;

namespace Wms.BarcodeLabel.Application.Contracts.Dtos;

public class PrintTaskOutputDto
{
    public Guid Id { get; set; }
    public string TaskNo { get; set; } = string.Empty;
    public string? PrinterId { get; set; }
    public string? PrinterName { get; set; }
    public Guid TemplateId { get; set; }
    public string? TemplateName { get; set; }
    public string SourceOrderType { get; set; } = string.Empty;
    public Guid SourceOrderId { get; set; }
    public string PrintContent { get; set; } = string.Empty;
    public int PrintQuantity { get; set; }
    public int PrintStatusValue { get; set; }
    public string PrintStatusName { get; set; } = string.Empty;
    public int RetryCount { get; set; }
    public string? ErrorMessage { get; set; }
    public DateTime? CompletedTime { get; set; }
}
