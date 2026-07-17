using System.ComponentModel.DataAnnotations;

namespace Wms.Material.Application.Contracts.Dtos;

/// <summary>
/// Unit of Measure Update DTO.
/// (API-MT-023, Phase 6 API Design)
/// </summary>
public class UnitOfMeasureUpdateDto
{
    [Required] [StringLength(100)] public string UnitName { get; set; } = string.Empty;
    [Required] [StringLength(20)] public string UnitSymbol { get; set; } = string.Empty;
    [Required] [Range(0, 6)] public int UnitType { get; set; }
    public bool IsActive { get; set; } = true;
}
