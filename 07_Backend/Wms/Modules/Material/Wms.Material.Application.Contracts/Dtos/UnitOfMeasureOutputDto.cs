namespace Wms.Material.Application.Contracts.Dtos;

/// <summary>
/// Unit of Measure Output DTO.
/// (API-MT-020~021, Phase 6 API Design)
/// </summary>
public class UnitOfMeasureOutputDto
{
    public Guid Id { get; set; }
    public string UnitCode { get; set; } = string.Empty;
    public string UnitName { get; set; } = string.Empty;
    public string UnitSymbol { get; set; } = string.Empty;
    public int UnitType { get; set; }
    public string UnitTypeDescription { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime CreationTime { get; set; }
}
