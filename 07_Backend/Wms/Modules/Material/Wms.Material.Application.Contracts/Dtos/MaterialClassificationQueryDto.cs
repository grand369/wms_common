using Volo.Abp.Application.Dtos;

namespace Wms.Material.Application.Contracts.Dtos;

/// <summary>
/// Material Classification Query DTO.
/// </summary>
public class MaterialClassificationQueryDto : PagedAndSortedResultRequestDto
{
    public string? ClassificationCode { get; set; }
    public string? ClassificationName { get; set; }
    public Guid? ParentClassificationId { get; set; }
    public int? ClassificationLevel { get; set; }
}
