namespace Wms.BarcodeLabel.Application.Contracts.Dtos;

public class BarcodeRuleUpdateDto
{
    public string? RuleName { get; set; }
    public string? CodePattern { get; set; }
    public string? Prefix { get; set; }
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;
}
