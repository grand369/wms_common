using System;

namespace Wms.BarcodeLabel.Application.Contracts.Dtos;

public class BarcodeResultDto
{
    public string? GeneratedCode { get; set; }
    public int? BarcodeTypeValue { get; set; }
    public Guid? RuleId { get; set; }
    public int? BarcodeFormatValue { get; set; }
}
