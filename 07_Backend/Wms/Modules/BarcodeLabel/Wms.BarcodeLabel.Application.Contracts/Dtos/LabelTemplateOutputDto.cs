using System;

namespace Wms.BarcodeLabel.Application.Contracts.Dtos;

public class LabelTemplateOutputDto
{
    public Guid Id { get; set; }
    public string TemplateName { get; set; } = string.Empty;
    public int TemplateTypeValue { get; set; }
    public string TemplateTypeName { get; set; } = string.Empty;
    public string TemplateContent { get; set; } = string.Empty;
    public int TemplateVersion { get; set; }
    public string? IndustryStandard { get; set; }
    public bool IsActive { get; set; }
}
