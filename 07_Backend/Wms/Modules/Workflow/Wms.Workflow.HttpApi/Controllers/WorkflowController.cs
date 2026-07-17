using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc;
using Wms.Workflow.Application.Contracts.Dtos;
using Wms.Workflow.Application.Contracts.Services;

namespace Wms.Workflow.HttpApi.Controllers;

/// <summary>
/// WorkflowController – REST API endpoints API-WF-001~010
/// Base route: /api/v1/workflow
/// </summary>
[RemoteService(Name = "WmsWorkflow")]
[Area("WmsWorkflow")]
[Route("api/v1/workflow")]
[Authorize]
public class WorkflowController : AbpControllerBase
{
    private readonly IWorkflowAppService _appService;

    public WorkflowController(IWorkflowAppService appService)
    {
        _appService = appService;
    }

    // ── Definition Endpoints ───────────────────────────────

    /// <summary>API-WF-001: GET /api/v1/workflow/definitions</summary>
    [HttpGet("definitions")]
    public Task<PagedResultDto<ApprovalFlowOutputDto>> GetDefinitionListAsync(ApprovalFlowQueryDto query)
        => _appService.GetDefinitionListAsync(query);

    /// <summary>API-WF-002: GET /api/v1/workflow/definitions/{id}</summary>
    [HttpGet("definitions/{id}")]
    public Task<ApprovalFlowOutputDto> GetDefinitionAsync(Guid id)
        => _appService.GetDefinitionAsync(id);

    /// <summary>API-WF-003: POST /api/v1/workflow/definitions</summary>
    [HttpPost("definitions")]
    public Task<ApprovalFlowOutputDto> CreateDefinitionAsync(ApprovalFlowCreateDto input)
        => _appService.CreateDefinitionAsync(input);

    /// <summary>API-WF-004: PUT /api/v1/workflow/definitions/{id}</summary>
    [HttpPut("definitions/{id}")]
    public Task<ApprovalFlowOutputDto> UpdateDefinitionAsync(Guid id, ApprovalFlowUpdateDto input)
        => _appService.UpdateDefinitionAsync(id, input);

    // ── Instance Endpoints ─────────────────────────────────

    /// <summary>API-WF-005: GET /api/v1/workflow/instances</summary>
    [HttpGet("instances")]
    public Task<PagedResultDto<ApprovalInstanceOutputDto>> GetInstanceListAsync(ApprovalInstanceQueryDto query)
        => _appService.GetInstanceListAsync(query);

    /// <summary>API-WF-006: GET /api/v1/workflow/instances/{id}</summary>
    [HttpGet("instances/{id}")]
    public Task<ApprovalInstanceOutputDto> GetInstanceAsync(Guid id)
        => _appService.GetInstanceAsync(id);

    // ── Business Operation Endpoints ───────────────────────

    /// <summary>API-WF-007: POST /api/v1/workflow/instances/start</summary>
    [HttpPost("instances/start")]
    public Task<ApprovalInstanceOutputDto> StartApprovalAsync(StartApprovalDto input)
        => _appService.StartApprovalAsync(input);

    /// <summary>API-WF-008: PATCH /api/v1/workflow/instances/{id}/approve</summary>
    [HttpPatch("instances/{id}/approve")]
    public Task<ApprovalInstanceOutputDto> ApproveAsync(Guid id, ApprovalActionDto input)
        => _appService.ApproveAsync(id, input);

    /// <summary>API-WF-009: PATCH /api/v1/workflow/instances/{id}/reject</summary>
    [HttpPatch("instances/{id}/reject")]
    public Task<ApprovalInstanceOutputDto> RejectAsync(Guid id, ApprovalActionDto input)
        => _appService.RejectAsync(id, input);

    /// <summary>API-WF-010: PATCH /api/v1/workflow/instances/{id}/resubmit</summary>
    [HttpPatch("instances/{id}/resubmit")]
    public Task<ApprovalInstanceOutputDto> ResubmitAsync(Guid id, ApprovalActionDto input)
        => _appService.ResubmitAsync(id, input);
}
