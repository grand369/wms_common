using System;
using Wms.Shared.Domain.Enums;
using Wms.TaskCenter.Domain.Enums;
using Wms.TaskCenter.Domain.Events;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.Domain.Entities;
using TaskStatus = Wms.TaskCenter.Domain.Enums.TaskStatus;

namespace Wms.TaskCenter.Domain.Aggregates;

/// <summary>
/// WarehouseTask Aggregate Root — AGG-14
/// Unified task lifecycle management for all warehouse operations.
/// State Machine SM-03: Created → Assigned → InProgress → Completed
///   + Suspended (from InProgress) + Cancelled (from Created/Assigned/Suspended)
/// REQ-TC-001~009
/// </summary>
public class WarehouseTask : FullAuditedAggregateRoot<Guid>
{
    // ── Identity ──
    public string TaskNo { get; private set; }

    // ── Type & Priority ──
    public TaskType TaskType { get; private set; }
    public TaskPriority TaskPriority { get; private set; }
    public TaskStatus TaskStatus { get; private set; }

    // ── Source Order (Polymorphic Association) ──
    public string SourceOrderType { get; private set; }
    public Guid SourceOrderId { get; private set; }
    public string SourceOrderNo { get; private set; }

    // ── Warehouse ──
    public Guid WarehouseId { get; private set; }
    public string WarehouseCode { get; private set; }

    // ── Assignment ──
    public Guid? AssignedUserId { get; private set; }
    public string? AssignedUserName { get; private set; }
    public AssignmentStrategy AssignmentStrategy { get; private set; }

    // ── Timing ──
    public DateTime? ExpectedCompletionTime { get; private set; }
    public DateTime? ActualStartTime { get; private set; }
    public DateTime? ActualCompletionTime { get; private set; }

    // ── Suspension ──
    public string? SuspendedReason { get; private set; }

    // ── Progress ──
    public decimal TaskProgress { get; private set; }

    // ── Remark ──
    public string? Remark { get; private set; }

    // ── EF Core constructor ──
    private WarehouseTask() { }

    public WarehouseTask(
        Guid id,
        string taskNo,
        TaskType taskType,
        TaskPriority taskPriority,
        string sourceOrderType,
        Guid sourceOrderId,
        string sourceOrderNo,
        Guid warehouseId,
        string warehouseCode,
        AssignmentStrategy assignmentStrategy,
        DateTime? expectedCompletionTime = null,
        string? remark = null) : base(id)
    {
        TaskNo = Check.NotNullOrWhiteSpace(taskNo, nameof(taskNo), maxLength: 50);
        TaskType = taskType ?? throw new ArgumentNullException(nameof(taskType));
        TaskPriority = taskPriority ?? TaskPriority.Medium;
        TaskStatus = TaskStatus.Created;
        SourceOrderType = Check.NotNullOrWhiteSpace(sourceOrderType, nameof(sourceOrderType), maxLength: 50);
        SourceOrderId = sourceOrderId;
        SourceOrderNo = Check.NotNullOrWhiteSpace(sourceOrderNo, nameof(sourceOrderNo), maxLength: 50);
        WarehouseId = warehouseId;
        WarehouseCode = Check.NotNullOrWhiteSpace(warehouseCode, nameof(warehouseCode), maxLength: 50);
        AssignmentStrategy = assignmentStrategy ?? AssignmentStrategy.Manual;
        ExpectedCompletionTime = expectedCompletionTime;
        TaskProgress = 0;
        Remark = remark;

        // DE-029: TaskCreatedEvent
        AddLocalEvent(new TaskCreatedEvent(id, taskType.Value, taskPriority.Value, sourceOrderId));
    }

    // ── SM-03: Assign ── Created → Assigned
    public void Assign(Guid userId, string userName, AssignmentStrategy? strategy = null)
    {
        if (TaskStatus != TaskStatus.Created && TaskStatus != TaskStatus.Assigned)
            throw new BusinessException("Wms.TaskCenter:TC-001", "任务状态不允许此操作，只有 Created 或 Assigned 状态才能分配。");

        if (TaskStatus == TaskStatus.Assigned && AssignedUserId == userId)
            throw new BusinessException("Wms.TaskCenter:TC-002", "任务已分配给该操作员，无需重复分配。");

        AssignedUserId = userId;
        AssignedUserName = Check.NotNullOrWhiteSpace(userName, nameof(userName), maxLength: 100);
        TaskStatus = TaskStatus.Assigned;
        if (strategy != null)
            AssignmentStrategy = strategy;

        // DE-030: TaskAssignedEvent
        AddLocalEvent(new TaskAssignedEvent(Id, userId, TaskType.Value));
    }

    // ── SM-03: Start ── Assigned → InProgress
    public void Start()
    {
        if (TaskStatus != TaskStatus.Assigned)
            throw new BusinessException("Wms.TaskCenter:TC-001", "任务状态不允许此操作，只有 Assigned 状态才能开始。");

        TaskStatus = TaskStatus.InProgress;
        ActualStartTime = DateTime.UtcNow;
        TaskProgress = 0;
    }

    // ── SM-03: Complete ── InProgress → Completed
    public void Complete()
    {
        if (TaskStatus != TaskStatus.InProgress)
            throw new BusinessException("Wms.TaskCenter:TC-001", "任务状态不允许此操作，只有 InProgress 状态才能完成。");

        TaskStatus = TaskStatus.Completed;
        ActualCompletionTime = DateTime.UtcNow;
        TaskProgress = 100;

        // DE-031: TaskCompletedEvent
        AddLocalEvent(new TaskCompletedEvent(Id, ActualCompletionTime!.Value));
    }

    // ── SM-03: Suspend ── InProgress → Suspended (reason required)
    public void Suspend(string reason)
    {
        if (TaskStatus != TaskStatus.InProgress)
            throw new BusinessException("Wms.TaskCenter:TC-001", "任务状态不允许此操作，只有 InProgress 状态才能挂起。");

        if (string.IsNullOrWhiteSpace(reason))
            throw new BusinessException("Wms.TaskCenter:TC-004", "挂起原因不能为空。");

        TaskStatus = TaskStatus.Suspended;
        SuspendedReason = Check.NotNullOrWhiteSpace(reason, nameof(reason), maxLength: 500);

        // DE-032: TaskSuspendedEvent
        AddLocalEvent(new TaskSuspendedEvent(Id, reason));
    }

    // ── SM-03: Resume ── Suspended → InProgress
    public void Resume()
    {
        if (TaskStatus != TaskStatus.Suspended)
            throw new BusinessException("Wms.TaskCenter:TC-001", "任务状态不允许此操作，只有 Suspended 状态才能恢复。");

        TaskStatus = TaskStatus.InProgress;
        SuspendedReason = null;
    }

    // ── SM-03: Cancel ── Created/Assigned/Suspended → Cancelled
    public void Cancel(string? reason = null)
    {
        if (TaskStatus == TaskStatus.InProgress || TaskStatus == TaskStatus.Completed)
            throw new BusinessException("Wms.TaskCenter:TC-001", "任务状态不允许此操作，进行中或已完成的任务不能取消。");

        TaskStatus = TaskStatus.Cancelled;
        Remark = reason ?? Remark;
    }

    // ── Reassign ── Assigned → Created (undo assignment)
    public void Reassign()
    {
        if (TaskStatus != TaskStatus.Assigned)
            throw new BusinessException("Wms.TaskCenter:TC-001", "任务状态不允许此操作，只有 Assigned 状态才能重新分配。");

        AssignedUserId = null;
        AssignedUserName = null;
        TaskStatus = TaskStatus.Created;
    }

    // ── Update Progress ──
    public void UpdateProgress(decimal progress)
    {
        if (TaskStatus != TaskStatus.InProgress)
            throw new BusinessException("Wms.TaskCenter:TC-001", "任务状态不允许此操作，只有 InProgress 状态才能更新进度。");

        if (progress < 0 || progress > 100)
            throw new BusinessException("Wms.TaskCenter:TC-ProgressOutOfRange", "任务进度必须在 0~100 之间。");

        TaskProgress = progress;
    }

    // ── Check Timeout ── DE-033: TaskTimeoutEvent
    public void CheckTimeout()
    {
        if (ExpectedCompletionTime == null) return;
        if (TaskStatus == TaskStatus.Completed || TaskStatus == TaskStatus.Cancelled) return;

        if (DateTime.UtcNow > ExpectedCompletionTime.Value)
        {
            // DE-033: TaskTimeoutEvent
            AddLocalEvent(new TaskTimeoutEvent(Id, ExpectedCompletionTime.Value));
        }
    }

    // ── Set Priority (BR-026: Emergency picking auto-elevate) ──
    public void SetPriority(TaskPriority priority)
    {
        TaskPriority = priority ?? throw new ArgumentNullException(nameof(priority));
    }

    // ── Set Expected Completion Time ──
    public void SetExpectedCompletionTime(DateTime expectedTime)
    {
        ExpectedCompletionTime = expectedTime;
    }
}
