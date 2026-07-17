namespace Wms.BarcodeLabel.Application.Contracts.Dtos;

public class LabelTemplateUpdateDto
{
    public string? TemplateName { get; set; }
    public string? TemplateContent { get; set; }
    public string? IndustryStandard { get; set; }
    public bool IsActive { get; set; } = true;
}
