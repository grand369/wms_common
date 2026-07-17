namespace Wms.Material.Application.Contracts.Dtos;

/// <summary>
/// Material Query DTO — query parameters for material list search.
/// (API-MT-001, Phase 6 API Design)
/// </summary>
public class MaterialQueryDto
{
    public string? MaterialCode { get; set; }
    public string? MaterialName { get; set; }
    public int? MaterialType { get; set; }
    public Guid? ClassificationId { get; set; }
    public bool? IsActive { get; set; }
    public int? ErpSyncStatus { get; set; }
    public int PageIndex { get; set; } = 0;
    public int PageSize { get; set; } = 10;
    public string? Sorting { get; set; }
}
