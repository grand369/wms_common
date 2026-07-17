using System.ComponentModel.DataAnnotations;

namespace Wms.Outbound.Application.Contracts.Dtos;

/// <summary>
/// DTO-OUT-001: OutboundOrderCreateDto — input DTO for creating an outbound order.
/// Contains nested Lines collection (DTO-NEST-001 pattern).
/// </summary>
public class OutboundOrderCreateDto
{
    /// <summary>Outbound type value (MaterialRequisition=1/SalesShipment=2/ReturnMaterial=3/TransferOutbound=4).</summary>
    [Range(1, 4)]
    public int OutboundTypeValue { get; set; }

    /// <summary>Source warehouse ID.</summary>
    [Required]
    public Guid WarehouseId { get; set; }

    /// <summary>Source warehouse code — redundant.</summary>
    [Required]
    [StringLength(50)]
    public string WarehouseCode { get; set; } = string.Empty;

    /// <summary>Material requisition ID — required when OutboundType = MaterialRequisition.</summary>
    public Guid? MaterialRequisitionId { get; set; }

    /// <summary>Sales order ID — required when OutboundType = SalesShipment.</summary>
    public Guid? SalesOrderId { get; set; }

    /// <summary>Return material order ID — required when OutboundType = ReturnMaterial.</summary>
    public Guid? ReturnMaterialOrderId { get; set; }

    /// <summary>Over-issue ratio — default 0.</summary>
    [Range(0, 1)]
    public decimal OverIssueRatio { get; set; } = 0m;

    /// <summary>Whether this is an emergency outbound order.</summary>
    public bool IsEmergency { get; set; } = false;

    /// <summary>Remark.</summary>
    [StringLength(1000)]
    public string? Remark { get; set; }

    /// <summary>Outbound lines — nested collection.</summary>
    [Required]
    [MinLength(1)]
    public List<OutboundLineCreateDto> Lines { get; set; } = new();
}

/// <summary>
/// OutboundLineCreateDto — input DTO for a single outbound line.
/// </summary>
public class OutboundLineCreateDto
{
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

    /// <summary>Required quantity — demand quantity.</summary>
    [Required]
    [Range(0.0001, double.MaxValue)]
    public decimal RequiredQuantity { get; set; }

    /// <summary>Issue strategy value (FIFO=0/FEFO=1/FMFO=2/Manual=3). Default 0 (FIFO).</summary>
    [Range(0, 3)]
    public int IssueStrategyValue { get; set; } = 0;

    /// <summary>Batch number — optional.</summary>
    [StringLength(50)]
    public string? BatchNumber { get; set; }

    /// <summary>Remark.</summary>
    [StringLength(500)]
    public string? Remark { get; set; }
}
