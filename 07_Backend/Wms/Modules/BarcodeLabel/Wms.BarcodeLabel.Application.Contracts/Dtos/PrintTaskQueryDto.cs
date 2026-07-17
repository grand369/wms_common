namespace Wms.BarcodeLabel.Application.Contracts.Dtos;

public class PrintTaskQueryDto
{
    public int? PrintStatusValue { get; set; }
    public string? PrinterId { get; set; }
    public string? SourceOrderType { get; set; }
    public int SkipCount { get; set; } = 0;
    public int MaxResultCount { get; set; } = 20;
}
