using System.ComponentModel.DataAnnotations;

namespace Wms.Outbound.Application.Contracts.Dtos;

/// <summary>
/// OutboundErpCallbackDto — ERP callback request DTO.
/// </summary>
public class OutboundErpCallbackDto
{
    /// <summary>ERP document number.</summary>
    [StringLength(100)]
    public string? ErpDocumentNo { get; set; }

    /// <summary>Callback status.</summary>
    [Required]
    public int CallbackStatus { get; set; }

    /// <summary>Callback message.</summary>
    [StringLength(500)]
    public string? Message { get; set; }

    /// <summary>Callback timestamp.</summary>
    public DateTime? CallbackTime { get; set; }
}
