using System.ComponentModel.DataAnnotations;

namespace Wms.Material.Application.Contracts.Dtos;

/// <summary>
/// Material Classification Create DTO.
/// (API-MT-016, Phase 6 API Design)
/// </summary>
public class MaterialClassificationCreateDto
{
    [Required] [StringLength(50)] public string ClassificationCode { get; set; } = string.Empty;
    [Required] [StringLength(200)] public string ClassificationName { get; set; } = string.Empty;
    public Guid? ParentClassificationId { get; set; }
    public int ClassificationLevel { get; set; } = 1;
    public Guid? AttributeTemplateId { get; set; }
}
