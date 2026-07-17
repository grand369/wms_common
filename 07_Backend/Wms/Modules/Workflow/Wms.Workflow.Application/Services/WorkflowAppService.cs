using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Application.Services;
using Wms.Workflow.Application.Contracts.Dtos;
using Wms.Workflow.Application.Contracts.Permissions;
using Wms.Workflow.Application.Contracts.Services;
using Wms.Workflow.Domain.Aggregates;
using Wms.Workflow.Domain.Enums;
using Wms.Workflow.Domain.Repositories;
using Wms.Workflow.Domain.Services;

namespace Wms.Workflow.Application.Services;

/// <summary>
/// WorkflowAppService — implements IWorkflowAppService (10 methods, API-WF-001~010)
/// </summary>
[Authorize(WmsWorkflowPermissions.Read)]
public class WorkflowAppService : ApplicationService, IWorkflowAppService
{
    private readonly IApprovalFlowRepository _flowRepository;
    private readonly IApprovalInstanceRepository _instanceRepository;
    private readonly WorkflowDomainService _domainService;

    public WorkflowAppService(
        IApprovalFlowRepository flowRepository,
        IApprovalInstanceRepository instanceRepository,
        WorkflowDomainService domainService)
    {
        _flowRepository = flowRepository;
        _instanceRepository = instanceRepository;
        _domainService = domainService;
    }

    // ── Definition CRUD ────────────────────────────────────

    /// <summary>API-WF-001: Get definition list with filtering</summary>
    public async Task<PagedResultDto<ApprovalFlowOutputDto>> GetDefinitionListAsync(ApprovalFlowQueryDto query)
    {
        var flows = await _flowRepository.GetListAsync();
        var filtered = flows.AsQueryable();

        if (query.FlowTypeValue.HasValue)
            filtered = filtered.Where(f => f.FlowType.Value == query.FlowTypeValue.Value);
        if (query.IsActive.HasValue)
            filtered = filtered.Where(f => f.IsActive == query.IsActive.Value);
        if (!string.IsNullOrEmpty(query.FlowName))
            filtered = filtered.Where(f => f.FlowName.Contains(query.FlowName));

        var result = filtered.ToList();
        var skip = query.SkipCount;
        var take = query.MaxResultCount > 0 ? query.MaxResultCount : result.Count;
        var paged = result.Skip(skip).Take(take).ToList();

        return new PagedResultDto<ApprovalFlowOutputDto>(
            result.Count,
            ObjectMapper.Map<List<ApprovalFlow>, List<ApprovalFlowOutputDto>>(paged));
    }

    /// <summary>API-WF-002: Get definition by id</summary>
    public async Task<ApprovalFlowOutputDto> GetDefinitionAsync(Guid id)
    {
        var flow = await _flowRepository.GetAsync(id);
        return ObjectMapper.Map<ApprovalFlow, ApprovalFlowOutputDto>(flow);
    }

    /// <summary>API-WF-003: Create definition</summary>
    [Authorize(WmsWorkflowPermissions.Create)]
    public async Task<ApprovalFlowOutputDto> CreateDefinitionAsync(ApprovalFlowCreateDto input)
    {
        var flowType = ApprovalFlowType.FromValue(input.FlowTypeValue);

        var flow = new ApprovalFlow(
            GuidGenerator.Create(),
            input.FlowName,
            flowType,
            input.Description);

        foreach (var nodeDto in input.Nodes.OrderBy(n => n.Order))
        {
            var nodeType = ApprovalNodeType.FromValue(nodeDto.NodeTypeValue);
            flow.AddNode(
                nodeDto.NodeName,
                nodeType,
                nodeDto.ApproverRole,
                nodeDto.ApproverUserId,
                nodeDto.ConditionExpression,
                nodeDto.Order,
                nodeDto.IsRequired);
        }

        await _flowRepository.InsertAsync(flow);
        return ObjectMapper.Map<ApprovalFlow, ApprovalFlowOutputDto>(flow);
    }

    /// <summary>API-WF-004: Update definition</summary>
    [Authorize(WmsWorkflowPermissions.Update)]
    public async Task<ApprovalFlowOutputDto> UpdateDefinitionAsync(Guid id, ApprovalFlowUpdateDto input)
    {
        var flow = await _flowRepository.GetAsync(id);

        // FlowName and Description have private setters (DDD); consider adding UpdateInfo domain method
        if (input.IsActive)
            flow.Activate();
        else
            flow.Deactivate();

        // Replace nodes if provided
        if (input.Nodes != null && input.Nodes.Count > 0)
        {
            // Remove existing nodes
            foreach (var existingNode in flow.Nodes.ToList())
                flow.RemoveNode(existingNode.Id);

            // Add new nodes
            foreach (var nodeDto in input.Nodes.OrderBy(n => n.Order))
            {
                var nodeType = ApprovalNodeType.FromValue(nodeDto.NodeTypeValue);
                flow.AddNode(
                    nodeDto.NodeName,
                    nodeType,
                    nodeDto.ApproverRole,
                    nodeDto.ApproverUserId,
                    nodeDto.ConditionExpression,
                    nodeDto.Order,
                    nodeDto.IsRequired);
            }
        }

        await _flowRepository.UpdateAsync(flow);
        return ObjectMapper.Map<ApprovalFlow, ApprovalFlowOutputDto>(flow);
    }

    // ── Instance Queries ───────────────────────────────────

    /// <summary>API-WF-005: Get instance list with filtering</summary>
    public async Task<PagedResultDto<ApprovalInstanceOutputDto>> GetInstanceListAsync(ApprovalInstanceQueryDto query)
    {
        var instances = await _instanceRepository.GetListAsync();
        var filtered = instances.AsQueryable();

        if (query.InstanceStatusValue.HasValue)
            filtered = filtered.Where(i => i.InstanceStatus.Value == query.InstanceStatusValue.Value);
        if (!string.IsNullOrEmpty(query.BusinessOrderType))
            filtered = filtered.Where(i => i.BusinessOrderType.Contains(query.BusinessOrderType));
        if (query.SubmitUserId.HasValue)
            filtered = filtered.Where(i => i.SubmitUserId == query.SubmitUserId.Value);

        var result = filtered.ToList();
        var skip = query.SkipCount;
        var take = query.MaxResultCount > 0 ? query.MaxResultCount : result.Count;
        var paged = result.Skip(skip).Take(take).ToList();

        return new PagedResultDto<ApprovalInstanceOutputDto>(
            result.Count,
            ObjectMapper.Map<List<ApprovalInstance>, List<ApprovalInstanceOutputDto>>(paged));
    }

    /// <summary>API-WF-006: Get instance by id</summary>
    public async Task<ApprovalInstanceOutputDto> GetInstanceAsync(Guid id)
    {
        var instance = await _instanceRepository.GetAsync(id);
        return ObjectMapper.Map<ApprovalInstance, ApprovalInstanceOutputDto>(instance);
    }

    // ── Business Operations ────────────────────────────────

    /// <summary>API-WF-007: Start approval</summary>
    [Authorize(WmsWorkflowPermissions.Execute)]
    public async Task<ApprovalInstanceOutputDto> StartApprovalAsync(StartApprovalDto input)
    {
        var instance = await _domainService.StartApprovalAsync(
            input.FlowId,
            input.BusinessOrderId,
            input.BusinessOrderType,
            input.BusinessOrderNo,
            CurrentUser.Id ?? Guid.Empty,
            CurrentUser.UserName);

        await _instanceRepository.InsertAsync(instance);
        return ObjectMapper.Map<ApprovalInstance, ApprovalInstanceOutputDto>(instance);
    }

    /// <summary>API-WF-008: Approve</summary>
    [Authorize(WmsWorkflowPermissions.Approve)]
    public async Task<ApprovalInstanceOutputDto> ApproveAsync(Guid id, ApprovalActionDto input)
    {
        var instance = await _domainService.ProcessApprovalAsync(
            id,
            CurrentUser.Id ?? Guid.Empty,
            CurrentUser.UserName,
            ApprovalActionType.Approve,
            input.Comment);

        await _instanceRepository.UpdateAsync(instance);
        return ObjectMapper.Map<ApprovalInstance, ApprovalInstanceOutputDto>(instance);
    }

    /// <summary>API-WF-009: Reject</summary>
    [Authorize(WmsWorkflowPermissions.Approve)]
    public async Task<ApprovalInstanceOutputDto> RejectAsync(Guid id, ApprovalActionDto input)
    {
        var instance = await _domainService.ProcessApprovalAsync(
            id,
            CurrentUser.Id ?? Guid.Empty,
            CurrentUser.UserName,
            ApprovalActionType.Reject,
            input.Comment);

        await _instanceRepository.UpdateAsync(instance);
        return ObjectMapper.Map<ApprovalInstance, ApprovalInstanceOutputDto>(instance);
    }

    /// <summary>API-WF-010: Resubmit</summary>
    [Authorize(WmsWorkflowPermissions.Execute)]
    public async Task<ApprovalInstanceOutputDto> ResubmitAsync(Guid id, ApprovalActionDto input)
    {
        var instance = await _domainService.ProcessApprovalAsync(
            id,
            CurrentUser.Id ?? Guid.Empty,
            CurrentUser.UserName,
            ApprovalActionType.Resubmit,
            input.Comment);

        await _instanceRepository.UpdateAsync(instance);
        return ObjectMapper.Map<ApprovalInstance, ApprovalInstanceOutputDto>(instance);
    }
}
