using System.ComponentModel.DataAnnotations;

namespace Wms.Warehouse.Application.Contracts.Dtos;

/// <summary>
/// Location Update DTO — request body for updating an existing location.
/// (API-WH-022, Phase 6 API Design)
/// </summary>
public class LocationUpdateDto
{
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
