using System;

namespace Wms.TaskCenter.Application.Contracts.Dtos;

/// <summary>
/// Output DTO for WarehouseTask — SmartEnum values flattened to int + description strings.
/// </summary>
public class WarehouseTaskOutputDto
{
    public Guid Id { get; set; }
    public string TaskNo { get; set; }

    // Type & Priority — flattened from SmartEnum
    public int TaskTypeValue { get; set; }
    public string TaskTypeDescription { get; set; }
    public int TaskPriorityValue { get; set; }
    public string TaskPriorityDescription { get; set; }
    public int TaskStatusValue { get; set; }
    public string TaskStatusDescription { get; set; }

    // Source Order
    public string SourceOrderType { get; set; }
    public Guid SourceOrderId { get; set; }
    public string SourceOrderNo { get; set; }

    // Warehouse
    public Guid WarehouseId { get; set; }
    public string WarehouseCode { get; set; }

    // Assignment
    public Guid? AssignedUserId { get; set; }
    public string? AssignedUserName { get; set; }
    public int AssignmentStrategyValue { get; set; }
    public string AssignmentStrategyDescription { get; set; }

    // Timing
    public DateTime? ExpectedCompletionTime { get; set; }
    public DateTime? ActualStartTime { get; set; }
    public DateTime? ActualCompletionTime { get; set; }

    // Suspension
    public string? SuspendedReason { get; set; }

    // Progress
    public decimal TaskProgress { get; set; }

    // Remark
    public string? Remark { get; set; }

    // Audit
    public DateTime CreationTime { get; set; }
    public Guid? CreatorId { get; set; }
    public DateTime? LastModificationTime { get; set; }
    public Guid? LastModifierId { get; set; }
}
