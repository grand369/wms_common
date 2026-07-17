using System;
using System.Linq;
using System.Threading.Tasks;
using Wms.Workflow.Domain.Aggregates;
using Wms.Workflow.Domain.Enums;
using Wms.Workflow.Domain.Repositories;

namespace Wms.Workflow.Domain.Services;

/// <summary>
/// DS-11: WorkflowDomainService — domain logic for approval workflow operations.
/// </summary>
public class WorkflowDomainService : DomainService
{
    private readonly IApprovalFlowRepository _flowRepository;
    private readonly IApprovalInstanceRepository _instanceRepository;

    public WorkflowDomainService(
        IApprovalFlowRepository flowRepository,
        IApprovalInstanceRepository instanceRepository)
    {
        _flowRepository = flowRepository;
        _instanceRepository = instanceRepository;
    }

    /// <summary>DS-11-01: Start an approval — create instance and advance to first approval node.</summary>
    public async Task<ApprovalInstance> StartApprovalAsync(
        Guid flowId,
        Guid businessOrderId,
        string businessOrderType,
        string? businessOrderNo,
        Guid submitUserId,
        string? submitUserName)
    {
        var flow = await _flowRepository.GetAsync(flowId);
        if (!flow.IsActive)
            throw new BusinessException("WMS:Workflow:0301", "Approval flow is not active.");

        // Check if an active instance already exists for this business order
        var existing = await _instanceRepository.GetByBusinessOrderAsync(businessOrderType, businessOrderId);
        if (existing != null &&
            existing.InstanceStatus != ApprovalInstanceStatus.Approved &&
            existing.InstanceStatus != ApprovalInstanceStatus.Rejected &&
            existing.InstanceStatus != ApprovalInstanceStatus.Cancelled)
            throw new BusinessException("WMS:Workflow:0302", "An active approval instance already exists for this business order.");

        var instance = new ApprovalInstance(
            GuidGenerator.Create(),
            flowId,
            flow.FlowName,
            businessOrderId,
            businessOrderType,
            businessOrderNo,
            submitUserId,
            submitUserName);

        // Advance to the first approval node
        var firstNode = flow.Nodes.OrderBy(n => n.Order).FirstOrDefault();
        if (firstNode != null)
        {
            instance.AdvanceToNode(
                firstNode.Id,
                firstNode.NodeName,
                firstNode.ApproverUserId,
                submitUserName);
        }

        return instance;
    }

    /// <summary>DS-11-02: Process an approval action — advance to next node or complete.</summary>
    public async Task<ApprovalInstance> ProcessApprovalAsync(
        Guid instanceId,
        Guid actionUserId,
        string? actionUserName,
        ApprovalActionType actionType,
        string? comment)
    {
        var instance = await _instanceRepository.GetAsync(instanceId);

        switch (actionType.Value)
        {
            case 0: // Approve
                return await ProcessApproveActionAsync(instance, actionUserId, comment);

            case 1: // Reject
                instance.Reject(actionUserId, comment);
                break;

            case 2: // Resubmit
                instance.Resubmit(comment);
                break;

            case 3: // Cancel
                instance.Cancel();
                break;

            default:
                throw new BusinessException("WMS:Workflow:0303", $"Unknown action type: {actionType.Name}.");
        }

        return instance;
    }

    /// <summary>DS-11-03: Cancel an approval instance.</summary>
    public async Task CancelApprovalAsync(Guid instanceId)
    {
        var instance = await _instanceRepository.GetAsync(instanceId);
        instance.Cancel();
    }

    // ── Private Helpers ─────────────────────────────────────────

    private async Task<ApprovalInstance> ProcessApproveActionAsync(
        ApprovalInstance instance,
        Guid actionUserId,
        string? comment)
    {
        instance.Approve(actionUserId, comment);

        // Load the flow to advance to next node
        var flow = await _flowRepository.GetAsync(instance.FlowId);

        var currentNodeId = instance.CurrentNodeId;
        var currentNode = flow.Nodes.FirstOrDefault(n => n.Id == currentNodeId);
        if (currentNode == null)
        {
            instance.CompleteApproval();
            return instance;
        }

        // Find the next node in order
        var nextNode = flow.Nodes
            .Where(n => n.Order > currentNode.Order)
            .OrderBy(n => n.Order)
            .FirstOrDefault();

        Guid? approverUserId = nextNode?.ApproverUserId;
        string? approverName = null;

        instance.AdvanceToNode(
            nextNode?.Id,
            nextNode?.NodeName,
            approverUserId,
            approverName);

        return instance;
    }
}
