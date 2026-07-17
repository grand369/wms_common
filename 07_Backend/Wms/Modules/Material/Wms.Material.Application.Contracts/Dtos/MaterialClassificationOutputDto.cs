namespace Wms.Material.Application.Contracts.Dtos;

/// <summary>
/// Material Classification Output DTO.
/// (API-MT-014~015, Phase 6 API Design)
/// </summary>
public class MaterialClassificationOutputDto
{
    public Guid Id { get; set; }
    public string ClassificationCode { get; set; } = string.Empty;
    public string ClassificationName { get; set; } = string.Empty;
    public Guid? ParentClassificationId { get; set; }
    public string? ParentClassificationName { get; set; }
    public int ClassificationLevel { get; set; }
    public Guid? AttributeTemplateId { get; set; }
    public List<MaterialClassificationOutputDto> Children { get; set; } = new();
    public DateTime CreationTime { get; set; }
}
