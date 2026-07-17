using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Wms.TaskCenter.Application.Contracts.Dtos;

/// <summary>
/// Command DTOs for WarehouseTask state transitions and operations.
/// API-TC-004~009, API-TC-012~014
/// </summary>

/// <summary> API-TC-004: Assign task to operator </summary>
public class TaskAssignCommandDto
{
    [Required]
    public Guid UserId { get; set; }

    [Required]
    [StringLength(100)]
    public string UserName { get; set; }

    public int? AssignmentStrategyValue { get; set; }
}

/// <summary> API-TC-006: Complete task </summary>
public class TaskCompleteCommandDto
{
    [StringLength(1000)]
    public string? Remark { get; set; }
}

/// <summary> API-TC-007: Suspend task (reason required — TC-004) </summary>
public class TaskSuspendCommandDto
{
    [Required]
    [StringLength(500)]
    public string Reason { get; set; }
}

/// <summary> API-TC-013: Update progress </summary>
public class TaskUpdateProgressCommandDto
{
    [Required]
    [Range(0, 100)]
    public decimal Progress { get; set; }
}

/// <summary> API-TC-012: Batch assign — multiple task IDs to one operator </summary>
public class TaskBatchAssignCommandDto
{
    [Required]
    public List<Guid> TaskIds { get; set; }

    [Required]
    public Guid UserId { get; set; }

    [Required]
    [StringLength(100)]
    public string UserName { get; set; }

    public int? AssignmentStrategyValue { get; set; }
}

/// <summary> API-TC-014: Auto-assign by strategy </summary>
public class TaskAutoAssignCommandDto
{
    [Required]
    public Guid WarehouseId { get; set; }

    [Required]
    [Range(0, 3)]
    public int AssignmentStrategyValue { get; set; }
}

/// <summary> API-TC-009: Cancel task </summary>
public class TaskCancelCommandDto
{
    [StringLength(500)]
    public string? Reason { get; set; }
}
