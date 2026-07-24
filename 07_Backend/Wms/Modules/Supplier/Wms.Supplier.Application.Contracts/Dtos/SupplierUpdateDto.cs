using System.ComponentModel.DataAnnotations;

namespace Wms.Supplier.Application.Contracts.Dtos;

/// <summary>
/// Supplier Update DTO — used to update an existing supplier.
/// </summary>
public class SupplierUpdateDto
{
    [Required]
    [StringLength(200)]
    public string SupplierName { get; set; } = string.Empty;

    [StringLength(100)]
    public string? ShortName { get; set; }

    [Range(1, 3)]
    public int SupplierType { get; set; } = 1;

    [StringLength(100)]
    public string? ContactName { get; set; }

    [StringLength(50)]
    public string? ContactPhone { get; set; }

    [EmailAddress]
    [StringLength(100)]
    public string? ContactEmail { get; set; }

    [StringLength(500)]
    public string? Address { get; set; }

    [StringLength(100)]
    public string? City { get; set; }

    [StringLength(100)]
    public string? Province { get; set; }

    [StringLength(20)]
    public string? PostalCode { get; set; }

    [StringLength(50)]
    public string? TaxId { get; set; }

    [StringLength(200)]
    public string? BankName { get; set; }

    [StringLength(50)]
    public string? BankAccount { get; set; }

    public bool IsActive { get; set; } = true;

    [StringLength(500)]
    public string? Remark { get; set; }

    [StringLength(50)]
    public string? ErpSupplierCode { get; set; }
}
