using System.ComponentModel.DataAnnotations;

namespace Wms.Material.Application.Contracts.Dtos;

/// <summary>
/// Material Classification Update DTO.
/// (API-MT-017, Phase 6 API Design)
/// </summary>
public class MaterialClassificationUpdateDto
{
    [Required] [StringLength(200)] public string ClassificationName { get; set; } = string.Empty;
    public Guid? ParentClassificationId { get; set; }
    public int ClassificationLevel { get; set; } = 1;
    public Guid? AttributeTemplateId { get; set; }
}
