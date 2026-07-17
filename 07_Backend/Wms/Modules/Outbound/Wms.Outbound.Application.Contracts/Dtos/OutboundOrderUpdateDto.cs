using System.ComponentModel.DataAnnotations;

namespace Wms.Outbound.Application.Contracts.Dtos;

/// <summary>
/// OutboundOrderUpdateDto — input DTO for updating an outbound order.
/// Only allowed in Draft status (OB-001).
/// </summary>
public class OutboundOrderUpdateDto
{
    /// <summary>Over-issue ratio.</summary>
    [Range(0, 1)]
    public decimal OverIssueRatio { get; set; }

    /// <summary>Whether this is an emergency outbound order.</summary>
    public bool IsEmergency { get; set; }

    /// <summary>Remark.</summary>
    [StringLength(1000)]
    public string? Remark { get; set; }
}
