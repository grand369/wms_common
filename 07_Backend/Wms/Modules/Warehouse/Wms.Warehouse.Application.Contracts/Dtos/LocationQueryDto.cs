namespace Wms.Warehouse.Application.Contracts.Dtos;

/// <summary>
/// Location Query DTO — query parameters for location list search.
/// (API-WH-019, Phase 6 API Design)
/// </summary>
public class LocationQueryDto
{
    public string? LocationCode { get; set; }
    public string? WarehouseId { get; set; }
    public string? AreaId { get; set; }
    public int? LocationType { get; set; }
    public int? StorageCondition { get; set; }
    public bool? IsActive { get; set; }
    public string? BarcodeId { get; set; }
    public int PageIndex { get; set; } = 0;
    public int PageSize { get; set; } = 10;
    public string? Sorting { get; set; }
}
