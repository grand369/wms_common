namespace Wms.Warehouse.Application.Contracts.Dtos;

/// <summary>
/// Warehouse Area Output DTO — response body for area queries.
/// (API-WH-013, Phase 6 API Design)
/// </summary>
public class WarehouseAreaOutputDto
{
    public Guid Id { get; set; }
    public string AreaCode { get; set; } = string.Empty;
    public string AreaName { get; set; } = string.Empty;
    public string WarehouseId { get; set; } = string.Empty;
    public string WarehouseCode { get; set; } = string.Empty;
    public int AreaFunction { get; set; }
    public string AreaFunctionDescription { get; set; } = string.Empty;
    public int StorageEnvironment { get; set; }
    public string StorageEnvironmentDescription { get; set; } = string.Empty;
    public decimal? MaxCapacity { get; set; }
    public decimal? CurrentCapacity { get; set; }
    public decimal? UtilizationPercentage { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreationTime { get; set; }
    public Guid? CreatorId { get; set; }
}
