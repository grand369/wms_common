namespace Wms.Warehouse.Application.Contracts.Dtos;

/// <summary>
/// Location Output DTO — response body for location queries.
/// (API-WH-020, Phase 6 API Design)
/// </summary>
public class LocationOutputDto
{
    public Guid Id { get; set; }
    public string LocationCode { get; set; } = string.Empty;
    public string WarehouseId { get; set; } = string.Empty;
    public string WarehouseCode { get; set; } = string.Empty;
    public string AreaId { get; set; } = string.Empty;
    public string AreaCode { get; set; } = string.Empty;
    public int LocationType { get; set; }
    public string LocationTypeDescription { get; set; } = string.Empty;
    public decimal? MaxWeight { get; set; }
    public decimal? MaxCapacity { get; set; }
    public decimal? CurrentWeight { get; set; }
    public decimal? CurrentCapacity { get; set; }
    public int StorageCondition { get; set; }
    public string StorageConditionDescription { get; set; } = string.Empty;
    public string BarcodeId { get; set; } = string.Empty;
    public string? Row { get; set; }
    public string? Column { get; set; }
    public string? Layer { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreationTime { get; set; }
    public Guid? CreatorId { get; set; }
}
