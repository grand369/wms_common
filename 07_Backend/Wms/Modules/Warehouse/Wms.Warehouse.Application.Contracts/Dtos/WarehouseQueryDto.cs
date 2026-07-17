namespace Wms.Warehouse.Application.Contracts.Dtos;

/// <summary>
/// Warehouse Query DTO — query parameters for warehouse list search.
/// (API-WH-001, Phase 6 API Design)
/// </summary>
public class WarehouseQueryDto
{
    public string? WarehouseCode { get; set; }
    public string? WarehouseName { get; set; }
    public int? WarehouseType { get; set; }
    public string? OrganizationUnitId { get; set; }
    public string? PlantId { get; set; }
    public bool? IsActive { get; set; }
    public int PageIndex { get; set; } = 0;
    public int PageSize { get; set; } = 10;
    public string? Sorting { get; set; }
}
