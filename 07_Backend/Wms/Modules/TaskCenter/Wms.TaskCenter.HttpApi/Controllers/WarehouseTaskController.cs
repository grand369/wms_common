using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Wms.TaskCenter.Application.Contracts.Dtos;
using Wms.TaskCenter.Application.Contracts.Permissions;
using Wms.TaskCenter.Application.Contracts.Services;

namespace Wms.TaskCenter.HttpApi.Controllers;

/// <summary>
/// WarehouseTaskController �?API-TC-001~014
/// 14 REST API endpoints for TaskCenter module.
/// Route prefix: /api/v1/task-center
/// </summary>
[RemoteService(Name = "WmsTaskCenter")]
[Area("WmsTaskCenter")]
[Route("api/v1/task-center/tasks")]
public class WarehouseTaskController : AbpControllerBase
{
    private readonly IWarehouseTaskAppService _appService;

    public WarehouseTaskController(IWarehouseTaskAppService appService)
    {
        _appService = appService;
    }

    // ── Query ──

    // API-TC-001: Task list
    [HttpGet]
    [Authorize(WmsTaskCenterPermissions.ReadList)]
    public Task<PagedResultDto<WarehouseTaskOutputDto>> GetListAsync(WarehouseTaskQueryDto input)
        => _appService.GetListAsync(input);

    // API-TC-002: Task detail
    [HttpGet("{id}")]
    [Authorize(WmsTaskCenterPermissions.ReadDetail)]
    public Task<WarehouseTaskOutputDto> GetAsync(Guid id)
        => _appService.GetAsync(id);

    // API-TC-010: My tasks
    [HttpGet("my-tasks")]
    [Authorize(WmsTaskCenterPermissions.ReadMyTasks)]
    public Task<PagedResultDto<WarehouseTaskOutputDto>> GetMyTasksAsync(WarehouseTaskQueryDto input)
        => _appService.GetMyTasksAsync(input);

    // API-TC-011: By source order
    [HttpGet("by-source-order")]
    [Authorize(WmsTaskCenterPermissions.ReadBySourceOrder)]
    public Task<List<WarehouseTaskOutputDto>> GetBySourceOrderAsync(string sourceOrderType, Guid sourceOrderId)
        => _appService.GetBySourceOrderAsync(sourceOrderType, sourceOrderId);

    // ── Create ──

    // API-TC-003: Create task
    [HttpPost]
    [Authorize(WmsTaskCenterPermissions.Create)]
    public Task<WarehouseTaskOutputDto> CreateAsync(WarehouseTaskCreateDto input)
        => _appService.CreateAsync(input);

    // ── State Transitions ──

    // API-TC-004: Assign
    [HttpPatch("{id}/assign")]
    [Authorize(WmsTaskCenterPermissions.AssignSingle)]
    public Task<WarehouseTaskOutputDto> AssignAsync(Guid id, TaskAssignCommandDto input)
        => _appService.AssignAsync(id, input);

    // API-TC-005: Start
    [HttpPatch("{id}/start")]
    [Authorize(WmsTaskCenterPermissions.ExecuteStart)]
    public Task<WarehouseTaskOutputDto> StartAsync(Guid id)
        => _appService.StartAsync(id);

    // API-TC-006: Complete
    [HttpPatch("{id}/complete")]
    [Authorize(WmsTaskCenterPermissions.ExecuteComplete)]
    public Task<WarehouseTaskOutputDto> CompleteAsync(Guid id, TaskCompleteCommandDto? input = null)
        => _appService.CompleteAsync(id, input);

    // API-TC-007: Suspend
    [HttpPatch("{id}/suspend")]
    [Authorize(WmsTaskCenterPermissions.SuspendTask)]
    public Task<WarehouseTaskOutputDto> SuspendAsync(Guid id, TaskSuspendCommandDto input)
        => _appService.SuspendAsync(id, input);

    // API-TC-008: Resume
    [HttpPatch("{id}/resume")]
    [Authorize(WmsTaskCenterPermissions.ResumeTask)]
    public Task<WarehouseTaskOutputDto> ResumeAsync(Guid id)
        => _appService.ResumeAsync(id);

    // API-TC-009: Cancel
    [HttpPatch("{id}/cancel")]
    [Authorize(WmsTaskCenterPermissions.Cancel)]
    public Task<WarehouseTaskOutputDto> CancelAsync(Guid id, TaskCancelCommandDto? input = null)
        => _appService.CancelAsync(id, input);

    // ── Batch & Auto ──

    // API-TC-012: Batch assign
    [HttpPost("batch-assign")]
    [Authorize(WmsTaskCenterPermissions.AssignBatch)]
    public Task<List<WarehouseTaskOutputDto>> BatchAssignAsync(TaskBatchAssignCommandDto input)
        => _appService.BatchAssignAsync(input);

    // API-TC-013: Update progress
    [HttpPatch("{id}/update-progress")]
    [Authorize(WmsTaskCenterPermissions.ExecuteUpdateProgress)]
    public Task<WarehouseTaskOutputDto> UpdateProgressAsync(Guid id, TaskUpdateProgressCommandDto input)
        => _appService.UpdateProgressAsync(id, input);

    // API-TC-014: Auto-assign
    [HttpPost("auto-assign")]
    [Authorize(WmsTaskCenterPermissions.AssignAuto)]
    public Task<List<WarehouseTaskOutputDto>> AutoAssignAsync(TaskAutoAssignCommandDto input)
        => _appService.AutoAssignAsync(input);
}
