using System.ComponentModel.DataAnnotations;

namespace Wms.Warehouse.Application.Contracts.Dtos;

/// <summary>
/// Warehouse Area Create DTO — request body for creating a new warehouse area.
/// (API-WH-014, Phase 6 API Design)
/// </summary>
public class WarehouseAreaCreateDto
{
    [Required]
    [StringLength(50, MinimumLength = 1)]
    public string AreaCode { get; set; } = string.Empty;

    [Required]
    [StringLength(200, MinimumLength = 1)]
    public string AreaName { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string WarehouseId { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string WarehouseCode { get; set; } = string.Empty;

    [Required]
    [Range(0, 5)]
    public int AreaFunction { get; set; }

    [Range(0, 4)]
    public int StorageEnvironment { get; set; } = 0;

    public decimal? MaxCapacity { get; set; }

    public decimal? CurrentCapacity { get; set; }

    public bool IsActive { get; set; } = true;
}
