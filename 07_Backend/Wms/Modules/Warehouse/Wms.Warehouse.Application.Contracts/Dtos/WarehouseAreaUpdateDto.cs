using System.ComponentModel.DataAnnotations;

namespace Wms.Warehouse.Application.Contracts.Dtos;

/// <summary>
/// Warehouse Area Update DTO — request body for updating an existing warehouse area.
/// (API-WH-015, Phase 6 API Design)
/// </summary>
public class WarehouseAreaUpdateDto
{
    [Required]
    [StringLength(200, MinimumLength = 1)]
    public string AreaName { get; set; } = string.Empty;

    [Required]
    [Range(0, 5)]
    public int AreaFunction { get; set; }

    [Range(0, 4)]
    public int StorageEnvironment { get; set; } = 0;

    public decimal? MaxCapacity { get; set; }

    public decimal? CurrentCapacity { get; set; }

    public bool IsActive { get; set; } = true;
}
