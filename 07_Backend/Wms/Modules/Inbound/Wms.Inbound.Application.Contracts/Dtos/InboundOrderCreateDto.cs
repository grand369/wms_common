using System.ComponentModel.DataAnnotations;

namespace Wms.Inbound.Application.Contracts.Dtos;

/// <summary>
/// DTO-IN-001: InboundOrderCreateDto — input DTO for creating an inbound order.
/// Contains nested Lines collection (DTO-NEST-001 pattern).
/// </summary>
public class InboundOrderCreateDto
{
    /// <summary>Inbound type value (PurchaseReceipt=1/ProductionReceipt=2/ReturnReceipt=3).</summary>
    [Range(1, 4)]
    public int InboundTypeValue { get; set; }

    /// <summary>Target warehouse ID.</summary>
    [Required]
    public Guid WarehouseId { get; set; }

    /// <summary>Target warehouse code — redundant.</summary>
    [Required]
    [StringLength(50)]
    public string WarehouseCode { get; set; } = string.Empty;

    /// <summary>Purchase order ID — required when InboundType = PurchaseReceipt.</summary>
    public Guid? PurchaseOrderId { get; set; }

    /// <summary>Purchase order number — redundant.</summary>
    [StringLength(50)]
    public string? PurchaseOrderNo { get; set; }

    /// <summary>Production order ID — required when InboundType = ProductionReceipt.</summary>
    public Guid? ProductionOrderId { get; set; }

    /// <summary>Return order ID — required when InboundType = ReturnReceipt.</summary>
    public Guid? ReturnOrderId { get; set; }

    /// <summary>Supplier ID — required when InboundType = PurchaseReceipt.</summary>
    public Guid? SupplierId { get; set; }

    /// <summary>Supplier name — redundant.</summary>
    [StringLength(100)]
    public string? SupplierName { get; set; }

    /// <summary>Over-receipt ratio — default 0.</summary>
    [Range(0, 1)]
    public decimal OverReceiptRatio { get; set; } = 0m;

    /// <summary>Whether quality inspection is required — default true.</summary>
    public bool QualityInspectionRequired { get; set; } = true;

    /// <summary>Remark.</summary>
    [StringLength(1000)]
    public string? Remark { get; set; }

    /// <summary>Inbound lines — nested collection.</summary>
    [Required]
    [MinLength(1)]
    public List<InboundLineCreateDto> Lines { get; set; } = new();
}

/// <summary>
/// InboundLineCreateDto — input DTO for a single inbound line.
/// </summary>
public class InboundLineCreateDto
{
    /// <summary>Line ID — for update scenarios.</summary>
    public Guid? Id { get; set; }

    /// <summary>Material ID.</summary>
    [Required]
    public Guid MaterialId { get; set; }

    /// <summary>Material code — redundant.</summary>
    [Required]
    [StringLength(50)]
    public string MaterialCode { get; set; } = string.Empty;

    /// <summary>Material name — redundant.</summary>
    [Required]
    [StringLength(200)]
    public string MaterialName { get; set; } = string.Empty;

    /// <summary>Unit of measure — redundant.</summary>
    [StringLength(50)]
    public string? Unit { get; set; }

    /// <summary>Plan quantity.</summary>
    [Required]
    [Range(0.0001, double.MaxValue)]
    public decimal PlanQuantity { get; set; }

    /// <summary>Putaway warehouse ID — optional.</summary>
    public Guid? PutawayWarehouseId { get; set; }

    /// <summary>Putaway warehouse code — redundant.</summary>
    [StringLength(50)]
    public string? PutawayWarehouseCode { get; set; }

    /// <summary>Putaway area ID — optional.</summary>
    public Guid? PutawayAreaId { get; set; }

    /// <summary>Putaway area code — redundant.</summary>
    [StringLength(50)]
    public string? PutawayAreaCode { get; set; }

    /// <summary>Putaway location ID — optional.</summary>
    public Guid? PutawayLocationId { get; set; }

    /// <summary>Putaway location code — redundant.</summary>
    [StringLength(50)]
    public string? PutawayLocationCode { get; set; }

    /// <summary>Batch number — optional.</summary>
    [StringLength(50)]
    public string? BatchNumber { get; set; }

    /// <summary>Expiry date — optional.</summary>
    public DateTime? ExpiryDate { get; set; }

    /// <summary>Production date — optional.</summary>
    public DateTime? ProductionDate { get; set; }

    /// <summary>Remark.</summary>
    [StringLength(500)]
    public string? Remark { get; set; }
}
