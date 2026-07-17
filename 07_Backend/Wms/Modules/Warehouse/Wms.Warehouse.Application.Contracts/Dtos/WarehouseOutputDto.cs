namespace Wms.Warehouse.Application.Contracts.Dtos;

/// <summary>
/// Warehouse Output DTO — response body for warehouse queries.
/// Follows DTO扁平冗余原则: all properties flattened including cross-aggregate redundant fields.
/// (API-WH-002, Phase 6 API Design)
/// </summary>
public class WarehouseOutputDto
{
    public Guid Id { get; set; }
    public string WarehouseCode { get; set; } = string.Empty;
    public string WarehouseName { get; set; } = string.Empty;
    public int WarehouseType { get; set; }
    public string WarehouseTypeDescription { get; set; } = string.Empty;
    public string OrganizationUnitId { get; set; } = string.Empty;
    public string OrganizationUnitName { get; set; } = string.Empty;
    public string PlantId { get; set; } = string.Empty;
    public string PlantName { get; set; } = string.Empty;
    public string? ResponsibleUserId { get; set; }
    public string? ResponsibleUserName { get; set; }
    public string? Address { get; set; }
    public int StorageConditionType { get; set; }
    public string StorageConditionTypeDescription { get; set; } = string.Empty;
    public int LocationLevelCount { get; set; }
    public bool IsActive { get; set; }
    public string? Remark { get; set; }
    public int? AreaCount { get; set; }
    public int? LocationCount { get; set; }
    public DateTime CreationTime { get; set; }
    public Guid? CreatorId { get; set; }
}
