namespace Wms.Supplier.Application.Contracts.Dtos;

/// <summary>
/// Supplier Query DTO — used for filtering and pagination.
/// </summary>
public class SupplierQueryDto
{
    public string? SupplierCode { get; set; }
    
    public string? SupplierName { get; set; }
    
    public string? Filter { get; set; }
    
    public int? SupplierType { get; set; }
    
    public bool? IsActive { get; set; }
    
    public int SkipCount { get; set; } = 0;
    
    public int MaxResultCount { get; set; } = 10;
    
    public string? Sorting { get; set; }
}
