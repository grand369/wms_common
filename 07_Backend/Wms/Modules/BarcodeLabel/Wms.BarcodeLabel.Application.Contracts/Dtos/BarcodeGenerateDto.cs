using System;

namespace Wms.BarcodeLabel.Application.Contracts.Dtos;

public class BarcodeGenerateDto
{
    public Guid RuleId { get; set; }
    public string? ReferenceId { get; set; }
}
