namespace Wms.BarcodeLabel.Application.Contracts.Dtos;

public class LabelTemplateQueryDto
{
    public int? TemplateTypeValue { get; set; }
    public bool? IsActive { get; set; }
    public int SkipCount { get; set; } = 0;
    public int MaxResultCount { get; set; } = 20;
}
