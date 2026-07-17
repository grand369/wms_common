namespace Wms.Warehouse.Application.Contracts.Dtos;

/// <summary>
/// Warehouse Area Query DTO — query parameters for area list search.
/// (API-WH-012, Phase 6 API Design)
/// </summary>
public class WarehouseAreaQueryDto
{
    public string? WarehouseId { get; set; }
    public string? AreaCode { get; set; }
    public string? AreaName { get; set; }
    public int? AreaFunction { get; set; }
    public int? StorageEnvironment { get; set; }
    public bool? IsActive { get; set; }
    public int PageIndex { get; set; } = 0;
    public int PageSize { get; set; } = 10;
    public string? Sorting { get; set; }
}
