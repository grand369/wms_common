namespace Wms.Material.Application.Contracts.Dtos;

/// <summary>
/// Unit of Measure Query DTO.
/// </summary>
public class UnitOfMeasureQueryDto
{
    public string? UnitCode { get; set; }
    public string? UnitName { get; set; }
    public int? UnitType { get; set; }
    public bool? IsActive { get; set; }
    public int PageIndex { get; set; } = 0;
    public int PageSize { get; set; } = 50;
    public string? Sorting { get; set; }
}
