using System;
using System.ComponentModel.DataAnnotations;
using Wms.Shared.Domain.Enums;

namespace Wms.TaskCenter.Application.Contracts.Dtos;

/// <summary>
/// DTO for creating a WarehouseTask (API-TC-003).
/// </summary>
public class WarehouseTaskCreateDto
{
    [Required]
    [StringLength(50)]
    public string TaskNo { get; set; }

    [Required]
    public int TaskTypeValue { get; set; }

    [Range(1, 4)]
    public int TaskPriorityValue { get; set; } = 2; // Medium

    [Required]
    [StringLength(50)]
    public string SourceOrderType { get; set; }

    [Required]
    public Guid SourceOrderId { get; set; }

    [Required]
    [StringLength(50)]
    public string SourceOrderNo { get; set; }

    [Required]
    public Guid WarehouseId { get; set; }

    [Required]
    [StringLength(50)]
    public string WarehouseCode { get; set; }

    [Range(0, 3)]
    public int AssignmentStrategyValue { get; set; } = 0; // Manual

    public DateTime? ExpectedCompletionTime { get; set; }

    [StringLength(1000)]
    public string? Remark { get; set; }
}
