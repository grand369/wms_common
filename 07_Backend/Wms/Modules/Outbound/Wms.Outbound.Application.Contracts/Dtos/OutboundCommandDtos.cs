using System.ComponentModel.DataAnnotations;

namespace Wms.Outbound.Application.Contracts.Dtos;

/// <summary>
/// OutboundAllocateCommandDto — API-OB-004 allocate inventory request DTO.
/// Contains line-level allocation details with location assignment.
/// </summary>
public class OutboundAllocateCommandDto
{
    /// <summary>Idempotency ID for duplicate request prevention.</summary>
    [Required]
    [StringLength(100)]
    public string IdempotencyId { get; set; } = string.Empty;

    /// <summary>Line-level allocation details.</summary>
    [Required]
    [MinLength(1)]
    public List<OutboundAllocateLineDto> Lines { get; set; } = new();
}

/// <summary>
/// OutboundAllocateLineDto — allocation detail for a single outbound line.
/// </summary>
public class OutboundAllocateLineDto
{
    /// <summary>Outbound line ID.</summary>
    [Required]
    public Guid LineId { get; set; }

    /// <summary>Allocated quantity.</summary>
    [Required]
    [Range(0.0001, double.MaxValue)]
    public decimal AllocatedQuantity { get; set; }

    /// <summary>Picking location ID — system recommended or manually specified.</summary>
    public Guid? LocationId { get; set; }

    /// <summary>Picking location code — redundant.</summary>
    [StringLength(50)]
    public string? LocationCode { get; set; }
}

/// <summary>
/// OutboundPickingCommandDto — API-OB-005 confirm picking request DTO.
/// </summary>
public class OutboundPickingCommandDto
{
    /// <summary>Idempotency ID.</summary>
    [Required]
    [StringLength(100)]
    public string IdempotencyId { get; set; } = string.Empty;

    /// <summary>Line-level picking details.</summary>
    [Required]
    [MinLength(1)]
    public List<OutboundPickingLineDto> Lines { get; set; } = new();
}

/// <summary>
/// OutboundPickingLineDto — picking detail for a single outbound line.
/// </summary>
public class OutboundPickingLineDto
{
    /// <summary>Outbound line ID.</summary>
    [Required]
    public Guid LineId { get; set; }

    /// <summary>Picked quantity.</summary>
    [Required]
    [Range(0, double.MaxValue)]
    public decimal PickedQuantity { get; set; }
}

/// <summary>
/// OutboundShippingCommandDto — API-OB-007 confirm shipping request DTO.
/// </summary>
public class OutboundShippingCommandDto
{
    /// <summary>Idempotency ID.</summary>
    [Required]
    [StringLength(100)]
    public string IdempotencyId { get; set; } = string.Empty;

    /// <summary>Line-level shipping details.</summary>
    [Required]
    [MinLength(1)]
    public List<OutboundShippingLineDto> Lines { get; set; } = new();
}

/// <summary>
/// OutboundShippingLineDto — shipping detail for a single outbound line.
/// </summary>
public class OutboundShippingLineDto
{
    /// <summary>Outbound line ID.</summary>
    [Required]
    public Guid LineId { get; set; }

    /// <summary>Shipped quantity — must not exceed picked quantity (OB-006).</summary>
    [Required]
    [Range(0, double.MaxValue)]
    public decimal ShippedQuantity { get; set; }
}
