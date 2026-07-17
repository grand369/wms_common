using System;
using System.ComponentModel.DataAnnotations;

namespace Wms.TaskCenter.Application.Contracts.Dtos;

/// <summary>
/// Query DTO for WarehouseTask list (API-TC-001).
/// Supports filtering by type, status, warehouse, priority, assigned user, and source order.
/// </summary>
public class WarehouseTaskQueryDto
{
    public int? TaskTypeValue { get; set; }
    public int? TaskStatusValue { get; set; }
    public Guid? WarehouseId { get; set; }
    public int? TaskPriorityValue { get; set; }
    public Guid? AssignedUserId { get; set; }
    public string? SourceOrderType { get; set; }
    public Guid? SourceOrderId { get; set; }
    public string? Keyword { get; set; }
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }

    // Paging
    [Range(0, int.MaxValue)]
    public int SkipCount { get; set; } = 0;
    [Range(1, 1000)]
    public int MaxResultCount { get; set; } = 20;
    public string? Sorting { get; set; }
}
