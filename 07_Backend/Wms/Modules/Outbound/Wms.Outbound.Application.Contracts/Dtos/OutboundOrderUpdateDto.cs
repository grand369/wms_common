using System.ComponentModel.DataAnnotations;

namespace Wms.Outbound.Application.Contracts.Dtos;

/// <summary>
/// OutboundOrderUpdateDto — input DTO for updating an outbound order.
/// Only allowed in Draft status (OB-001).
/// </summary>
public class OutboundOrderUpdateDto
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
    public List<OutboundLineCreateDto> Lines { get; set; } = new();
}
