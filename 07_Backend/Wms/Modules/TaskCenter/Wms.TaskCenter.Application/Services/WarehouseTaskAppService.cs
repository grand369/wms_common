using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp;
using Volo.Abp.Application.Services;
using Wms.Shared.Domain.Enums;
using Wms.TaskCenter.Application.Contracts.Dtos;
using Wms.TaskCenter.Application.Contracts.Permissions;
using Wms.TaskCenter.Application.Contracts.Services;
using Wms.TaskCenter.Domain.Enums;
using TaskStatus = Wms.TaskCenter.Domain.Enums.TaskStatus;
using Wms.TaskCenter.Domain.Aggregates;
using Wms.TaskCenter.Domain.Repositories;
using Wms.TaskCenter.Domain.Services;

namespace Wms.TaskCenter.Application.Services;

/// <summary>
/// WarehouseTaskAppService — 14 API methods (API-TC-001~014)
/// Implements IWarehouseTaskAppService with full CRUD + state transitions + batch/auto operations.
/// </summary>
[Authorize(WmsTaskCenterPermissions.Read)]
public class WarehouseTaskAppService :
    ApplicationService,
    IWarehouseTaskAppService
{
    private readonly IWarehouseTaskRepository _taskRepository;
    private readonly TaskDomainService _taskDomainService;

    public WarehouseTaskAppService(
        IWarehouseTaskRepository taskRepository,
        TaskDomainService taskDomainService)
    {
        _taskRepository = taskRepository;
        _taskDomainService = taskDomainService;
    }

    // ── Query ──

    // API-TC-001: Task list
    public async Task<PagedResultDto<WarehouseTaskOutputDto>> GetListAsync(WarehouseTaskQueryDto input)
    {
        var query = await _taskRepository.GetQueryableAsync();

        query = query.Where(t => !t.IsDeleted);

        if (input.TaskTypeValue.HasValue)
            query = query.Where(t => t.TaskType.Value == input.TaskTypeValue.Value);
        if (input.TaskStatusValue.HasValue)
            query = query.Where(t => t.TaskStatus.Value == input.TaskStatusValue.Value);
        if (input.WarehouseId.HasValue)
            query = query.Where(t => t.WarehouseId == input.WarehouseId.Value);
        if (input.TaskPriorityValue.HasValue)
            query = query.Where(t => t.TaskPriority.Value == input.TaskPriorityValue.Value);
        if (input.AssignedUserId.HasValue)
            query = query.Where(t => t.AssignedUserId == input.AssignedUserId.Value);
        if (!string.IsNullOrWhiteSpace(input.SourceOrderType))
            query = query.Where(t => t.SourceOrderType == input.SourceOrderType);
        if (input.SourceOrderId.HasValue)
            query = query.Where(t => t.SourceOrderId == input.SourceOrderId.Value);
        if (!string.IsNullOrWhiteSpace(input.Keyword))
            query = query.Where(t => t.TaskNo.Contains(input.Keyword) || t.SourceOrderNo.Contains(input.Keyword));
        if (input.StartTime.HasValue)
            query = query.Where(t => t.CreationTime >= input.StartTime.Value);
        if (input.EndTime.HasValue)
            query = query.Where(t => t.CreationTime <= input.EndTime.Value);

        var totalCount = await AsyncExecuter.CountAsync(query);

        query = query.OrderByDescending(t => t.TaskPriority.Value)
                      .ThenBy(t => t.CreationTime);

        var items = await AsyncExecuter.ToListAsync(
            query.Skip(input.SkipCount).Take(input.MaxResultCount));

        return new PagedResultDto<WarehouseTaskOutputDto>(
            totalCount,
            items.Select(MapToOutputDto).ToList());
    }

    // API-TC-002: Task detail
    public async Task<WarehouseTaskOutputDto> GetAsync(Guid id)
    {
        var task = await _taskRepository.GetAsync(id);
        return MapToOutputDto(task);
    }

    // API-TC-010: My tasks (current user)
    [Authorize(WmsTaskCenterPermissions.ReadMyTasks)]
    public async Task<PagedResultDto<WarehouseTaskOutputDto>> GetMyTasksAsync(WarehouseTaskQueryDto input)
    {
        var currentUserId = CurrentUser.Id ?? Guid.Empty;
        var tasks = await _taskRepository.GetByAssignedUserAsync(currentUserId);

        // Additional filters
        var filtered = tasks.AsQueryable();
        if (input.TaskTypeValue.HasValue)
            filtered = filtered.Where(t => t.TaskType.Value == input.TaskTypeValue.Value);
        if (input.TaskStatusValue.HasValue)
            filtered = filtered.Where(t => t.TaskStatus.Value == input.TaskStatusValue.Value);

        var totalCount = filtered.Count();
        var items = filtered
            .OrderByDescending(t => t.TaskPriority.Value)
            .ThenBy(t => t.CreationTime)
            .Skip(input.SkipCount)
            .Take(input.MaxResultCount)
            .Select(MapToOutputDto)
            .ToList();

        return new PagedResultDto<WarehouseTaskOutputDto>(totalCount, items);
    }

    // API-TC-011: Tasks by source order
    public async Task<List<WarehouseTaskOutputDto>> GetBySourceOrderAsync(string sourceOrderType, Guid sourceOrderId)
    {
        var tasks = await _taskRepository.GetBySourceOrderAsync(sourceOrderType, sourceOrderId);
        return tasks.Select(MapToOutputDto).ToList();
    }

    // ── Create ──

    // API-TC-003: Create task
    [Authorize(WmsTaskCenterPermissions.Create)]
    public async Task<WarehouseTaskOutputDto> CreateAsync(WarehouseTaskCreateDto input)
    {
        var taskType = TaskType.FromValue(input.TaskTypeValue);
        var priority = TaskPriority.FromValue(input.TaskPriorityValue);
        var strategy = AssignmentStrategy.FromValue(input.AssignmentStrategyValue);

        var task = await _taskDomainService.CreateTaskFromOrderAsync(
            taskType,
            input.SourceOrderId,
            input.SourceOrderType,
            input.SourceOrderNo,
            input.WarehouseId,
            input.WarehouseCode,
            priority,
            strategy,
            input.ExpectedCompletionTime);

        return MapToOutputDto(task);
    }

    // ── State Transitions ──

    // API-TC-004: Assign task
    [Authorize(WmsTaskCenterPermissions.AssignSingle)]
    public async Task<WarehouseTaskOutputDto> AssignAsync(Guid id, TaskAssignCommandDto input)
    {
        var strategy = input.AssignmentStrategyValue.HasValue
            ? AssignmentStrategy.FromValue(input.AssignmentStrategyValue.Value)
            : null;

        var task = await _taskRepository.GetAsync(id);
        task.Assign(input.UserId, input.UserName, strategy);
        await _taskRepository.UpdateAsync(task);

        return MapToOutputDto(task);
    }

    // API-TC-005: Start task (PDA)
    [Authorize(WmsTaskCenterPermissions.ExecuteStart)]
    public async Task<WarehouseTaskOutputDto> StartAsync(Guid id)
    {
        var task = await _taskRepository.GetAsync(id);
        task.Start();
        await _taskRepository.UpdateAsync(task);

        return MapToOutputDto(task);
    }

    // API-TC-006: Complete task
    [Authorize(WmsTaskCenterPermissions.ExecuteComplete)]
    public async Task<WarehouseTaskOutputDto> CompleteAsync(Guid id, TaskCompleteCommandDto? input = null)
    {
        var task = await _taskRepository.GetAsync(id);
        task.Complete();
        // task.Remark cannot be set directly (DDD private setter); consider adding UpdateRemark domain method
        await _taskRepository.UpdateAsync(task);

        return MapToOutputDto(task);
    }

    // API-TC-007: Suspend task
    [Authorize(WmsTaskCenterPermissions.SuspendTask)]
    public async Task<WarehouseTaskOutputDto> SuspendAsync(Guid id, TaskSuspendCommandDto input)
    {
        var task = await _taskDomainService.SuspendTaskAsync(id, input.Reason);
        return MapToOutputDto(task);
    }

    // API-TC-008: Resume task
    [Authorize(WmsTaskCenterPermissions.ResumeTask)]
    public async Task<WarehouseTaskOutputDto> ResumeAsync(Guid id)
    {
        var task = await _taskDomainService.ResumeTaskAsync(id);
        return MapToOutputDto(task);
    }

    // API-TC-009: Cancel task
    [Authorize(WmsTaskCenterPermissions.Cancel)]
    public async Task<WarehouseTaskOutputDto> CancelAsync(Guid id, TaskCancelCommandDto? input = null)
    {
        var task = await _taskRepository.GetAsync(id);
        task.Cancel(input?.Reason);
        await _taskRepository.UpdateAsync(task);

        return MapToOutputDto(task);
    }

    // ── Batch & Auto ──

    // API-TC-012: Batch assign
    [Authorize(WmsTaskCenterPermissions.AssignBatch)]
    public async Task<List<WarehouseTaskOutputDto>> BatchAssignAsync(TaskBatchAssignCommandDto input)
    {
        var strategy = input.AssignmentStrategyValue.HasValue
            ? AssignmentStrategy.FromValue(input.AssignmentStrategyValue.Value)
            : null;

        var results = new List<WarehouseTaskOutputDto>();
        foreach (var taskId in input.TaskIds)
        {
            var task = await _taskRepository.GetAsync(taskId);
            task.Assign(input.UserId, input.UserName, strategy);
            await _taskRepository.UpdateAsync(task);
            results.Add(MapToOutputDto(task));
        }
        return results;
    }

    // API-TC-013: Update progress
    [Authorize(WmsTaskCenterPermissions.ExecuteUpdateProgress)]
    public async Task<WarehouseTaskOutputDto> UpdateProgressAsync(Guid id, TaskUpdateProgressCommandDto input)
    {
        var task = await _taskRepository.GetAsync(id);
        task.UpdateProgress(input.Progress);
        await _taskRepository.UpdateAsync(task);

        return MapToOutputDto(task);
    }

    // API-TC-014: Auto-assign by strategy
    [Authorize(WmsTaskCenterPermissions.AssignAuto)]
    public async Task<List<WarehouseTaskOutputDto>> AutoAssignAsync(TaskAutoAssignCommandDto input)
    {
        var strategy = AssignmentStrategy.FromValue(input.AssignmentStrategyValue);
        var tasks = await _taskDomainService.AutoAssignTasksAsync(input.WarehouseId, strategy);
        return tasks.Select(MapToOutputDto).ToList();
    }

    // ── Mapping ──
    private WarehouseTaskOutputDto MapToOutputDto(WarehouseTask task)
    {
        return new WarehouseTaskOutputDto
        {
            Id = task.Id,
            TaskNo = task.TaskNo,
            TaskTypeValue = task.TaskType.Value,
            TaskTypeDescription = task.TaskType.Description,
            TaskPriorityValue = task.TaskPriority.Value,
            TaskPriorityDescription = task.TaskPriority.Description,
            TaskStatusValue = task.TaskStatus.Value,
            TaskStatusDescription = task.TaskStatus.Description,
            SourceOrderType = task.SourceOrderType,
            SourceOrderId = task.SourceOrderId,
            SourceOrderNo = task.SourceOrderNo,
            WarehouseId = task.WarehouseId,
            WarehouseCode = task.WarehouseCode,
            AssignedUserId = task.AssignedUserId,
            AssignedUserName = task.AssignedUserName,
            AssignmentStrategyValue = task.AssignmentStrategy.Value,
            AssignmentStrategyDescription = task.AssignmentStrategy.Description,
            ExpectedCompletionTime = task.ExpectedCompletionTime,
            ActualStartTime = task.ActualStartTime,
            ActualCompletionTime = task.ActualCompletionTime,
            SuspendedReason = task.SuspendedReason,
            TaskProgress = task.TaskProgress,
            Remark = task.Remark,
            CreationTime = task.CreationTime,
            CreatorId = task.CreatorId,
            LastModificationTime = task.LastModificationTime,
            LastModifierId = task.LastModifierId,
        };
    }
}
