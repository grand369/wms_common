using System;
using System.Collections.Generic;
using System.Linq;
using Volo.Abp.Domain.Entities;
using Wms.Workflow.Domain.Enums;
using Wms.Workflow.Domain.Events;

namespace Wms.Workflow.Domain.Aggregates;

/// <summary>
/// AGG-25: ApprovalInstance Aggregate Root
/// Represents a running instance of an approval flow for a specific business order.
/// </summary>
public class ApprovalInstance : FullAuditedAggregateRoot<Guid>
{
    // ── Properties ──────────────────────────────────────────────
    public Guid FlowId { get; private set; }
    public string? FlowName { get; private set; }
    public ApprovalInstanceStatus InstanceStatus { get; private set; }
    public Guid BusinessOrderId { get; private set; }
    public string BusinessOrderType { get; private set; }
    public string? BusinessOrderNo { get; private set; }
    public Guid? CurrentNodeId { get; private set; }
    public string? CurrentNodeName { get; private set; }
    public Guid SubmitUserId { get; private set; }
    public string? SubmitUserName { get; private set; }
    public DateTime SubmitTime { get; private set; }
    public DateTime? CompletedTime { get; private set; }

    // ── Navigation ──────────────────────────────────────────────
    public List<ApprovalActionLog> ActionLogs { get; private set; } = new();

    // ── Constructors ────────────────────────────────────────────
    protected ApprovalInstance() { } // EF Core

    public ApprovalInstance(
        Guid id,
        Guid flowId,
        string? flowName,
        Guid businessOrderId,
        string businessOrderType,
        string? businessOrderNo,
        Guid submitUserId,
        string? submitUserName)
    {
        Id = id;
        FlowId = flowId;
        FlowName = flowName;
        BusinessOrderId = businessOrderId;
        BusinessOrderType = businessOrderType ?? throw new ArgumentNullException(nameof(businessOrderType));
        BusinessOrderNo = businessOrderNo;
        SubmitUserId = submitUserId;
        SubmitUserName = submitUserName;
        SubmitTime = DateTime.UtcNow;
        InstanceStatus = ApprovalInstanceStatus.Pending;
    }

    // ── Domain Methods ──────────────────────────────────────────

    /// <summary>Approve the current node and advance.</summary>
    public void Approve(Guid actionUserId, string? comment = null)
    {
        if (InstanceStatus != ApprovalInstanceStatus.Pending &&
            InstanceStatus != ApprovalInstanceStatus.InProgress &&
            InstanceStatus != ApprovalInstanceStatus.Resubmitted)
            throw new BusinessException("WMS:Workflow:0201", "Only Pending, InProgress, or Resubmitted instances can be approved.");

        AddActionLog(actionUserId, ApprovalActionType.Approve, comment);
    }

    /// <summary>Reject the current instance.</summary>
    public void Reject(Guid actionUserId, string? comment = null)
    {
        if (InstanceStatus != ApprovalInstanceStatus.Pending &&
            InstanceStatus != ApprovalInstanceStatus.InProgress &&
            InstanceStatus != ApprovalInstanceStatus.Resubmitted)
            throw new BusinessException("WMS:Workflow:0202", "Only Pending, InProgress, or Resubmitted instances can be rejected.");

        InstanceStatus = ApprovalInstanceStatus.Rejected;
        CompletedTime = DateTime.UtcNow;
        AddActionLog(actionUserId, ApprovalActionType.Reject, comment);

        AddLocalEvent(new ApprovalCompletedEvent(
            Id, FlowId, BusinessOrderId, BusinessOrderType, "Rejected"));
        AddLocalEvent(new ApprovalRejectedEvent(
            Id, FlowId, BusinessOrderId, BusinessOrderType, comment ?? string.Empty));
    }

    /// <summary>Resubmit after rejection.</summary>
    public void Resubmit(string? comment = null)
    {
        if (InstanceStatus != ApprovalInstanceStatus.Rejected)
            throw new BusinessException("WMS:Workflow:0203", "Only Rejected instances can be resubmitted.");

        InstanceStatus = ApprovalInstanceStatus.Resubmitted;
        AddActionLog(SubmitUserId, ApprovalActionType.Resubmit, comment);
    }

    /// <summary>Cancel the approval instance.</summary>
    public void Cancel()
    {
        if (InstanceStatus == ApprovalInstanceStatus.Approved ||
            InstanceStatus == ApprovalInstanceStatus.Cancelled)
            throw new BusinessException("WMS:Workflow:0204", "Approved or already cancelled instances cannot be cancelled.");

        InstanceStatus = ApprovalInstanceStatus.Cancelled;
        CompletedTime = DateTime.UtcNow;
    }

    // ── Internal helpers ────────────────────────────────────────

    /// <summary>Advance to the next node or mark as completed.</summary>
    internal void AdvanceToNode(Guid? nextNodeId, string? nextNodeName, Guid? approverUserId, string? approverName)
    {
        if (nextNodeId == null)
        {
            CompleteApproval();
            return;
        }

        CurrentNodeId = nextNodeId;
        CurrentNodeName = nextNodeName;
        InstanceStatus = ApprovalInstanceStatus.InProgress;

        if (approverUserId.HasValue)
        {
            AddLocalEvent(new ApprovalPendingEvent(
                Id, FlowId, BusinessOrderId, BusinessOrderType,
                approverUserId.Value, approverName ?? string.Empty));
        }
    }

    /// <summary>Complete the approval successfully.</summary>
    internal void CompleteApproval()
    {
        InstanceStatus = ApprovalInstanceStatus.Approved;
        CompletedTime = DateTime.UtcNow;

        AddLocalEvent(new ApprovalCompletedEvent(
            Id, FlowId, BusinessOrderId, BusinessOrderType, "Approved"));
    }

    /// <summary>Record initial pending event after start.</summary>
    internal void NotifyPending(Guid approverId, string? approverName)
    {
        AddLocalEvent(new ApprovalPendingEvent(
            Id, FlowId, BusinessOrderId, BusinessOrderType,
            approverId, approverName ?? string.Empty));
    }

    private void AddActionLog(Guid actionUserId, ApprovalActionType actionType, string? comment)
    {
        var log = new ApprovalActionLog(
            Guid.NewGuid(),
            Id,
            CurrentNodeId ?? Guid.Empty,
            CurrentNodeName,
            actionUserId,
            SubmitUserName, // uses submit user name as fallback
            actionType,
            comment,
            DateTime.UtcNow);
        ActionLogs.Add(log);
    }
}

/// <summary>
/// ApprovalActionLog — sub-entity of ApprovalInstance (AGG-25).
/// Records each action taken during the approval process.
/// </summary>
public class ApprovalActionLog : Entity<Guid>
{
    // ── Properties ──────────────────────────────────────────────
    public Guid InstanceId { get; private set; }
    public Guid NodeId { get; private set; }
    public string? NodeName { get; private set; }
    public Guid ActionUserId { get; private set; }
    public string? ActionUserName { get; private set; }
    public ApprovalActionType ActionType { get; private set; }
    public string? Comment { get; private set; }
    public DateTime ActionTime { get; private set; }

    // ── Constructors ────────────────────────────────────────────
    protected ApprovalActionLog() { } // EF Core

    public ApprovalActionLog(
        Guid id,
        Guid instanceId,
        Guid nodeId,
        string? nodeName,
        Guid actionUserId,
        string? actionUserName,
        ApprovalActionType actionType,
        string? comment,
        DateTime actionTime)
    {
        Id = id;
        InstanceId = instanceId;
        NodeId = nodeId;
        NodeName = nodeName;
        ActionUserId = actionUserId;
        ActionUserName = actionUserName;
        ActionType = actionType ?? throw new ArgumentNullException(nameof(actionType));
        Comment = comment;
        ActionTime = actionTime;
    }
}
