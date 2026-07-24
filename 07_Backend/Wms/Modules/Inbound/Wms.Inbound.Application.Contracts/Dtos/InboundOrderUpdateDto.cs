using System.ComponentModel.DataAnnotations;

namespace Wms.Inbound.Application.Contracts.Dtos;

/// <summary>
/// InboundOrderUpdateDto — input DTO for updating an inbound order.
/// Only allowed in Draft status (IN-001).
/// </summary>
public class InboundOrderUpdateDto
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

    /// <summary>Over-receipt ratio.</summary>
    [Range(0, 1)]
    public decimal OverReceiptRatio { get; set; }

    /// <summary>Whether quality inspection is required.</summary>
    public bool QualityInspectionRequired { get; set; }

    /// <summary>Remark.</summary>
    [StringLength(1000)]
    public string? Remark { get; set; }

    /// <summary>Inbound lines — nested collection.</summary>
    public List<InboundLineCreateDto> Lines { get; set; } = new();
}
