using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Services;
using Wms.Shared.Domain.Enums;
using Wms.Shared.Domain.Interfaces;
using Wms.TaskCenter.Domain.Aggregates;
using Wms.TaskCenter.Domain.Enums;
using TaskStatus = Wms.TaskCenter.Domain.Enums.TaskStatus;
using Wms.TaskCenter.Domain.Repositories;

namespace Wms.TaskCenter.Domain.Services;

/// <summary>
/// TaskDomainService — DS-05
/// Domain service for WarehouseTask lifecycle operations.
/// Implements ITaskDomainService for cross-module DI (CROSS-003).
/// REQ-TC-001~009
/// </summary>
[Dependency(ReplaceServices = true)]
[ExposeServices(typeof(ITaskDomainService), typeof(TaskDomainService))]
public class TaskDomainService : DomainService, ITaskDomainService
{
    private readonly IWarehouseTaskRepository _taskRepository;

    public TaskDomainService(IWarehouseTaskRepository taskRepository)
    {
        _taskRepository = taskRepository;
    }

    /// <summary>
    /// Create a task from a source order — REQ-TC-009
    /// Used by Inbound/Outbound/Transfer to auto-generate tasks on order confirmation.
    /// </summary>
    public async Task<WarehouseTask> CreateTaskFromOrderAsync(
        TaskType taskType,
        Guid sourceOrderId,
        string sourceOrderType,
        string sourceOrderNo,
        Guid warehouseId,
        string warehouseCode,
        TaskPriority priority,
        AssignmentStrategy strategy,
        DateTime? expectedCompletionTime = null,
        string? taskNo = null)
    {
        // Check if tasks already exist for this source order
        var existingTasks = await _taskRepository.GetBySourceOrderAsync(sourceOrderType, sourceOrderId);
        var activeTasks = existingTasks.Where(t =>
            t.TaskStatus != TaskStatus.Completed && t.TaskStatus != TaskStatus.Cancelled).ToList();

        if (activeTasks.Any())
        {
            throw new BusinessException("Wms.TaskCenter:TC-DuplicateTask",
                $"来源单据 {sourceOrderNo} 已存在活跃任务，不能重复创建。");
        }

        var generatedTaskNo = taskNo ?? $"TC-{taskType.Name}-{sourceOrderNo}-{Clock.Now:yyyyMMddHHmmss}";
        var task = new WarehouseTask(
            GuidGenerator.Create(),
            generatedTaskNo,
            taskType,
            priority,
            sourceOrderType,
            sourceOrderId,
            sourceOrderNo,
            warehouseId,
            warehouseCode,
            strategy,
            expectedCompletionTime);

        return await _taskRepository.InsertAsync(task);
    }

    /// <summary>
    /// Assign a task to an operator — REQ-TC-005 (manual assignment)
    /// </summary>
    public async Task<WarehouseTask> AssignTaskAsync(Guid taskId, Guid userId, string userName)
    {
        var task = await _taskRepository.GetAsync(taskId);
        task.Assign(userId, userName);
        return await _taskRepository.UpdateAsync(task);
    }

    /// <summary>
    /// Auto-assign pending tasks based on strategy — REQ-TC-005 (auto assignment)
    /// BR-028: Priority-based ordering (Emergency > High > Medium > Low)
    /// </summary>
    public async Task<List<WarehouseTask>> AutoAssignTasksAsync(
        Guid warehouseId, AssignmentStrategy strategy)
    {
        var pendingTasks = await _taskRepository.GetPendingAssignmentAsync(warehouseId);

        // BR-028: Sort by priority descending, then by creation time ascending
        var ordered = pendingTasks
            .OrderByDescending(t => t.TaskPriority.Value)
            .ThenBy(t => t.CreationTime)
            .ToList();

        // Auto-assignment logic placeholder (v1.1: Region/Skill/LoadBalance strategies)
        // For now, just return the ordered list — actual assignment depends on operator pool
        return ordered;
    }

    /// <summary>
    /// Suspend a task — REQ-TC-003
    /// </summary>
    public async Task<WarehouseTask> SuspendTaskAsync(Guid taskId, string reason)
    {
        var task = await _taskRepository.GetAsync(taskId);
        task.Suspend(reason);
        return await _taskRepository.UpdateAsync(task);
    }

    /// <summary>
    /// Resume a suspended task — REQ-TC-003
    /// </summary>
    public async Task<WarehouseTask> ResumeTaskAsync(Guid taskId)
    {
        var task = await _taskRepository.GetAsync(taskId);
        task.Resume();
        return await _taskRepository.UpdateAsync(task);
    }

    /// <summary>
    /// Complete a task — REQ-TC-001
    /// </summary>
    public async Task<WarehouseTask> CompleteTaskAsync(Guid taskId)
    {
        var task = await _taskRepository.GetAsync(taskId);
        task.Complete();
        return await _taskRepository.UpdateAsync(task);
    }

    /// <summary>
    /// Check timeout for all active tasks — REQ-TC-007
    /// </summary>
    public async Task<List<WarehouseTask>> CheckTaskTimeoutAsync()
    {
        var timeoutTasks = await _taskRepository.GetTimeoutTasksAsync();
        foreach (var task in timeoutTasks)
        {
            task.CheckTimeout();
        }
        return timeoutTasks;
    }

    // ── ITaskDomainService explicit implementations (CROSS-003) ──

    /// <summary>
    /// ITaskDomainService.CreateTaskFromOrderAsync — cross-module adapter.
    /// Maps int-based SmartEnum values to domain enum types.
    /// </summary>
    async Task<Guid> ITaskDomainService.CreateTaskFromOrderAsync(
        int taskTypeValue,
        Guid sourceOrderId,
        string sourceOrderType,
        string sourceOrderNo,
        Guid warehouseId,
        string warehouseCode,
        int priorityValue,
        int assignmentStrategyValue,
        DateTime? expectedCompletionTime)
    {
        var taskType = TaskType.FromValue(taskTypeValue);
        var priority = TaskPriority.FromValue(priorityValue);
        var strategy = AssignmentStrategy.FromValue(assignmentStrategyValue);

        var task = await CreateTaskFromOrderAsync(
            taskType, sourceOrderId, sourceOrderType, sourceOrderNo,
            warehouseId, warehouseCode, priority, strategy, expectedCompletionTime);

        return task.Id;
    }

    /// <summary>
    /// ITaskDomainService.CancelTasksBySourceOrderAsync — cross-module adapter.
    /// Cancels all active tasks associated with a source order.
    /// </summary>
    async Task ITaskDomainService.CancelTasksBySourceOrderAsync(string sourceOrderType, Guid sourceOrderId)
    {
        var existingTasks = await _taskRepository.GetBySourceOrderAsync(sourceOrderType, sourceOrderId);
        var activeTasks = existingTasks.Where(t =>
            t.TaskStatus != TaskStatus.Completed && t.TaskStatus != TaskStatus.Cancelled).ToList();

        foreach (var task in activeTasks)
        {
            task.Cancel();
            await _taskRepository.UpdateAsync(task);
        }
    }
}
