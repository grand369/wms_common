namespace Wms.BarcodeLabel.Application.Contracts.Dtos;

public class BarcodeRuleQueryDto
{
    public int? BarcodeTypeValue { get; set; }
    public bool? IsActive { get; set; }
    public string? RuleName { get; set; }
    public int SkipCount { get; set; } = 0;
    public int MaxResultCount { get; set; } = 20;
}
