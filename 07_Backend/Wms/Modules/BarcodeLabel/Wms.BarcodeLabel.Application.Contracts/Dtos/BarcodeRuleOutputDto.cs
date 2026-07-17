using System;

namespace Wms.BarcodeLabel.Application.Contracts.Dtos;

public class BarcodeRuleOutputDto
{
    public Guid Id { get; set; }
    public string RuleName { get; set; } = string.Empty;
    public int BarcodeTypeValue { get; set; }
    public string BarcodeTypeName { get; set; } = string.Empty;
    public int BarcodeFormatValue { get; set; }
    public string BarcodeFormatName { get; set; } = string.Empty;
    public string CodePattern { get; set; } = string.Empty;
    public string? Prefix { get; set; }
    public string? Description { get; set; }
    public bool IsActive { get; set; }
    public int SeqCounter { get; set; }
}
