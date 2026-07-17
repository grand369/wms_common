using System.ComponentModel.DataAnnotations;

namespace Wms.Inbound.Application.Contracts.Dtos;

/// <summary>
/// InboundPutawayCommandDto — API-IN-008 putaway confirmation request DTO.
/// </summary>
public class InboundPutawayCommandDto
{
    /// <summary>Idempotency ID.</summary>
    [Required]
    [StringLength(100)]
    public string IdempotencyId { get; set; } = string.Empty;

    /// <summary>Line-level putaway details.</summary>
    [Required]
    [MinLength(1)]
    public List<InboundPutawayLineDto> Lines { get; set; } = new();
}

/// <summary>
/// InboundPutawayLineDto — putaway detail for a single inbound line.
/// </summary>
public class InboundPutawayLineDto
{
    /// <summary>Inbound line ID.</summary>
    [Required]
    public Guid LineId { get; set; }

    /// <summary>Putaway location ID.</summary>
    [Required]
    public Guid PutawayLocationId { get; set; }

    /// <summary>Putaway location code — redundant.</summary>
    [Required]
    [StringLength(50)]
    public string PutawayLocationCode { get; set; } = string.Empty;

    /// <summary>Putaway quantity.</summary>
    [Required]
    [Range(0.0001, double.MaxValue)]
    public decimal Quantity { get; set; }
}
