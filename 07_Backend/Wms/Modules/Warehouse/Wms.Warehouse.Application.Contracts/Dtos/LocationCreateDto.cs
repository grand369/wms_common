using System.ComponentModel.DataAnnotations;

namespace Wms.Warehouse.Application.Contracts.Dtos;

/// <summary>
/// Location Create DTO — request body for creating a new location.
/// (API-WH-021, Phase 6 API Design)
/// </summary>
public class LocationCreateDto
{
    [Required]
    [StringLength(50, MinimumLength = 1)]
    public string LocationCode { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string WarehouseId { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string WarehouseCode { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string AreaId { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string AreaCode { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string BarcodeId { get; set; } = string.Empty;

    [Range(0, 4)]
    public int LocationType { get; set; } = 0;

    [Range(0, 4)]
    public int StorageCondition { get; set; } = 0;

    public decimal? MaxWeight { get; set; }
    public decimal? MaxCapacity { get; set; }

    [StringLength(10)]
    public string? Row { get; set; }

    [StringLength(10)]
    public string? Column { get; set; }

    [StringLength(10)]
    public string? Layer { get; set; }

    public bool IsActive { get; set; } = true;
}
