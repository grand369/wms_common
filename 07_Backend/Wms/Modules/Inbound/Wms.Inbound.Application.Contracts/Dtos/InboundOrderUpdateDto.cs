using System.ComponentModel.DataAnnotations;

namespace Wms.Inbound.Application.Contracts.Dtos;

/// <summary>
/// InboundOrderUpdateDto — input DTO for updating an inbound order.
/// Only allowed in Draft status (IN-001).
/// </summary>
public class InboundOrderUpdateDto
{
    /// <summary>Over-receipt ratio.</summary>
    [Range(0, 1)]
    public decimal OverReceiptRatio { get; set; }

    /// <summary>Whether quality inspection is required.</summary>
    public bool QualityInspectionRequired { get; set; }

    /// <summary>Remark.</summary>
    [StringLength(1000)]
    public string? Remark { get; set; }
}
