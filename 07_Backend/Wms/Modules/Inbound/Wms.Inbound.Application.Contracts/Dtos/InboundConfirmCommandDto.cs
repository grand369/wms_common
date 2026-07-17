using System.ComponentModel.DataAnnotations;

namespace Wms.Inbound.Application.Contracts.Dtos;

/// <summary>
/// InboundConfirmCommandDto — API-IN-006 confirm receipt request DTO.
/// Contains idempotency key and line-level receipt details.
/// </summary>
public class InboundConfirmCommandDto
{
    /// <summary>Idempotency ID for duplicate request prevention.</summary>
    [Required]
    [StringLength(100)]
    public string IdempotencyId { get; set; } = string.Empty;

    /// <summary>Line-level receipt details.</summary>
    [Required]
    [MinLength(1)]
    public List<InboundConfirmLineDto> Lines { get; set; } = new();
}

/// <summary>
/// InboundConfirmLineDto — receipt detail for a single inbound line.
/// </summary>
public class InboundConfirmLineDto
{
    /// <summary>Inbound line ID.</summary>
    [Required]
    public Guid LineId { get; set; }

    /// <summary>Received quantity.</summary>
    [Required]
    [Range(0.0001, double.MaxValue)]
    public decimal ReceivedQuantity { get; set; }

    /// <summary>Batch number — optional override.</summary>
    [StringLength(50)]
    public string? BatchNumber { get; set; }
}
