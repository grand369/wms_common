using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Wms.TaskCenter.Application.Contracts.Dtos;

namespace Wms.TaskCenter.Application.Contracts.Services;

/// <summary>
/// IWarehouseTaskAppService — API-TC-001~014
/// 14 API methods for WarehouseTask lifecycle management.
/// </summary>
public interface IWarehouseTaskAppService : IApplicationService
{
    // ── Query ──
    // API-TC-001: Task list
    Task<PagedResultDto<WarehouseTaskOutputDto>> GetListAsync(WarehouseTaskQueryDto input);

    // API-TC-002: Task detail
    Task<WarehouseTaskOutputDto> GetAsync(Guid id);

    // API-TC-010: My tasks (current user)
    Task<PagedResultDto<WarehouseTaskOutputDto>> GetMyTasksAsync(WarehouseTaskQueryDto input);

    // API-TC-011: Tasks by source order
    Task<List<WarehouseTaskOutputDto>> GetBySourceOrderAsync(string sourceOrderType, Guid sourceOrderId);

    // ── Create ──
    // API-TC-003: Create task
    Task<WarehouseTaskOutputDto> CreateAsync(WarehouseTaskCreateDto input);

    // ── State Transitions ──
    // API-TC-004: Assign task
    Task<WarehouseTaskOutputDto> AssignAsync(Guid id, TaskAssignCommandDto input);

    // API-TC-005: Start task (PDA)
    Task<WarehouseTaskOutputDto> StartAsync(Guid id);

    // API-TC-006: Complete task
    Task<WarehouseTaskOutputDto> CompleteAsync(Guid id, TaskCompleteCommandDto? input = null);

    // API-TC-007: Suspend task
    Task<WarehouseTaskOutputDto> SuspendAsync(Guid id, TaskSuspendCommandDto input);

    // API-TC-008: Resume task
    Task<WarehouseTaskOutputDto> ResumeAsync(Guid id);

    // API-TC-009: Cancel task
    Task<WarehouseTaskOutputDto> CancelAsync(Guid id, TaskCancelCommandDto? input = null);

    // ── Batch & Auto ──
    // API-TC-012: Batch assign
    Task<List<WarehouseTaskOutputDto>> BatchAssignAsync(TaskBatchAssignCommandDto input);

    // API-TC-013: Update progress
    Task<WarehouseTaskOutputDto> UpdateProgressAsync(Guid id, TaskUpdateProgressCommandDto input);

    // API-TC-014: Auto-assign by strategy
    Task<List<WarehouseTaskOutputDto>> AutoAssignAsync(TaskAutoAssignCommandDto input);
}
