using System.ComponentModel.DataAnnotations;

namespace Wms.Warehouse.Application.Contracts.Dtos;

/// <summary>
/// Warehouse Update DTO — request body for updating an existing warehouse.
/// (API-WH-004, Phase 6 API Design)
/// </summary>
public class WarehouseUpdateDto
{
    [Required]
    [StringLength(200, MinimumLength = 1)]
    public string WarehouseName { get; set; } = string.Empty;

    [Required]
    [Range(0, 11)]
    public int WarehouseType { get; set; }

    [Required]
    [StringLength(200)]
    public string OrganizationUnitName { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string PlantName { get; set; } = string.Empty;

    public string? ResponsibleUserId { get; set; }

    [StringLength(100)]
    public string? ResponsibleUserName { get; set; }

    [StringLength(500)]
    public string? Address { get; set; }

    [Range(0, 4)]
    public int StorageConditionType { get; set; } = 0;

    [Required]
    [Range(3, 4)]
    public int LocationLevelCount { get; set; } = 3;

    public bool IsActive { get; set; } = true;

    [StringLength(1000)]
    public string? Remark { get; set; }
}
