namespace Wms.Supplier.Application.Contracts.Dtos;

/// <summary>
/// Supplier Output DTO — represents supplier data for display purposes.
/// </summary>
public class SupplierOutputDto
{
    public Guid Id { get; set; }
    
    public string SupplierCode { get; set; } = string.Empty;
    
    public string SupplierName { get; set; } = string.Empty;
    
    public string? ShortName { get; set; }
    
    public int SupplierType { get; set; }
    
    public string? SupplierTypeDescription { get; set; }
    
    public string? ContactName { get; set; }
    
    public string? ContactPhone { get; set; }
    
    public string? ContactEmail { get; set; }
    
    public string? Address { get; set; }
    
    public string? City { get; set; }
    
    public string? Province { get; set; }
    
    public string? PostalCode { get; set; }
    
    public string? TaxId { get; set; }
    
    public string? BankName { get; set; }
    
    public string? BankAccount { get; set; }
    
    public bool IsActive { get; set; }
    
    public string? Remark { get; set; }
    
    public string? ErpSupplierCode { get; set; }
    
    public DateTime CreationTime { get; set; }
    
    public Guid? CreatorId { get; set; }
}
