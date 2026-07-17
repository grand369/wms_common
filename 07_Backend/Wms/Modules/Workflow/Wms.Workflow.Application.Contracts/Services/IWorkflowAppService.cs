using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Wms.Workflow.Application.Contracts.Dtos;

namespace Wms.Workflow.Application.Contracts.Services;

/// <summary>
/// IWorkflowAppService — 10 API methods (API-WF-001~010)
/// </summary>
public interface IWorkflowAppService : IApplicationService
{
    // ── Definition CRUD ────────────────────────────────────
    /// <summary>API-WF-001: Get definition list</summary>
    Task<PagedResultDto<ApprovalFlowOutputDto>> GetDefinitionListAsync(ApprovalFlowQueryDto query);

    /// <summary>API-WF-002: Get definition by id</summary>
    Task<ApprovalFlowOutputDto> GetDefinitionAsync(Guid id);

    /// <summary>API-WF-003: Create definition</summary>
    Task<ApprovalFlowOutputDto> CreateDefinitionAsync(ApprovalFlowCreateDto input);

    /// <summary>API-WF-004: Update definition</summary>
    Task<ApprovalFlowOutputDto> UpdateDefinitionAsync(Guid id, ApprovalFlowUpdateDto input);

    // ── Instance Queries ───────────────────────────────────
    /// <summary>API-WF-005: Get instance list</summary>
    Task<PagedResultDto<ApprovalInstanceOutputDto>> GetInstanceListAsync(ApprovalInstanceQueryDto query);

    /// <summary>API-WF-006: Get instance by id</summary>
    Task<ApprovalInstanceOutputDto> GetInstanceAsync(Guid id);

    // ── Business Operations ────────────────────────────────
    /// <summary>API-WF-007: Start approval</summary>
    Task<ApprovalInstanceOutputDto> StartApprovalAsync(StartApprovalDto input);

    /// <summary>API-WF-008: Approve</summary>
    Task<ApprovalInstanceOutputDto> ApproveAsync(Guid id, ApprovalActionDto input);

    /// <summary>API-WF-009: Reject</summary>
    Task<ApprovalInstanceOutputDto> RejectAsync(Guid id, ApprovalActionDto input);

    /// <summary>API-WF-010: Resubmit</summary>
    Task<ApprovalInstanceOutputDto> ResubmitAsync(Guid id, ApprovalActionDto input);
}
