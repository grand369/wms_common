using System;
using System.ComponentModel.DataAnnotations;

namespace Wms.TaskCenter.Application.Contracts.Dtos;

/// <summary>
/// DTO for updating a WarehouseTask (only in Created status).
/// </summary>
public class WarehouseTaskUpdateDto
{
    [Range(1, 4)]
    public int TaskPriorityValue { get; set; }

    [Range(0, 3)]
    public int AssignmentStrategyValue { get; set; }

    public DateTime? ExpectedCompletionTime { get; set; }

    [StringLength(1000)]
    public string? Remark { get; set; }
}
