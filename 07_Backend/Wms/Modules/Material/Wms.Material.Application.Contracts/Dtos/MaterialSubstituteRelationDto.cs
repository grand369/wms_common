namespace Wms.Material.Application.Contracts.Dtos;

/// <summary>
/// Material Substitute Relation DTO — represents a substitute material relation.
/// (API-MT-010, Phase 6 API Design)
/// </summary>
public class MaterialSubstituteRelationDto
{
    public Guid Id { get; set; }
    public Guid OriginalMaterialId { get; set; }
    public Guid SubstituteMaterialId { get; set; }
    public string SubstituteMaterialCode { get; set; } = string.Empty;
    public string? SubstituteMaterialName { get; set; }
    public int SubstitutePriority { get; set; }
    public decimal SubstituteRatio { get; set; }
}
