namespace Wms.BarcodeLabel.Application.Contracts.Dtos;

public class LabelTemplateCreateDto
{
    public string TemplateName { get; set; } = string.Empty;
    public int TemplateTypeValue { get; set; }
    public string TemplateContent { get; set; } = string.Empty;
    public string? IndustryStandard { get; set; }
}
