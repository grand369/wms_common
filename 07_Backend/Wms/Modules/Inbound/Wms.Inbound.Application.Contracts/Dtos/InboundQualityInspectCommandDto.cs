using System.ComponentModel.DataAnnotations;

namespace Wms.Inbound.Application.Contracts.Dtos;

/// <summary>
/// InboundQualityInspectCommandDto — API-IN-007 quality inspection request DTO.
/// </summary>
public class InboundQualityInspectCommandDto
{
    /// <summary>Idempotency ID.</summary>
    [Required]
    [StringLength(100)]
    public string IdempotencyId { get; set; } = string.Empty;

    /// <summary>Line-level quality inspection details.</summary>
    [Required]
    [MinLength(1)]
    public List<InboundQualityInspectLineDto> Lines { get; set; } = new();
}

/// <summary>
/// InboundQualityInspectLineDto — quality inspection detail for a single inbound line.
/// </summary>
public class InboundQualityInspectLineDto
{
    /// <summary>Inbound line ID.</summary>
    [Required]
    public Guid LineId { get; set; }

    /// <summary>Quality result — 1=Qualified, 2=Unqualified, 3=Skip.</summary>
    [Required]
    [Range(1, 3)]
    public int QualityResultValue { get; set; }
}
