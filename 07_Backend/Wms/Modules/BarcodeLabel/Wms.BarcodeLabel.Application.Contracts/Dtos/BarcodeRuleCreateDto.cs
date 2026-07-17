namespace Wms.BarcodeLabel.Application.Contracts.Dtos;

public class BarcodeRuleCreateDto
{
    public string RuleName { get; set; } = string.Empty;
    public int BarcodeTypeValue { get; set; }
    public int BarcodeFormatValue { get; set; }
    public string CodePattern { get; set; } = string.Empty;
    public string? Prefix { get; set; }
    public string? Description { get; set; }
}
