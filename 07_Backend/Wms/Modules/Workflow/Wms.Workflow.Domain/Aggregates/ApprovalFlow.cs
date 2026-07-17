using System;
using System.Collections.Generic;
using System.Linq;
using Volo.Abp.Domain.Entities;
using Wms.Workflow.Domain.Enums;

namespace Wms.Workflow.Domain.Aggregates;

/// <summary>
/// AGG-24: ApprovalFlow Aggregate Root
/// Defines an approval flow template with ordered approval nodes.
/// </summary>
public class ApprovalFlow : FullAuditedAggregateRoot<Guid>
{
    // ── Properties ──────────────────────────────────────────────
    public string FlowName { get; private set; }
    public ApprovalFlowType FlowType { get; private set; }
    public bool IsActive { get; private set; }
    public string? Description { get; private set; }

    // ── Navigation ──────────────────────────────────────────────
    public List<ApprovalNode> Nodes { get; private set; } = new();

    // ── Constructors ────────────────────────────────────────────
    protected ApprovalFlow() { } // EF Core

    public ApprovalFlow(
        Guid id,
        string flowName,
        ApprovalFlowType flowType,
        string? description = null)
    {
        Id = id;
        FlowName = flowName ?? throw new ArgumentNullException(nameof(flowName));
        FlowType = flowType ?? throw new ArgumentNullException(nameof(flowType));
        IsActive = true;
        Description = description;
    }

    // ── Domain Methods ──────────────────────────────────────────

    /// <summary>Add an approval node to the flow.</summary>
    public ApprovalNode AddNode(
        string nodeName,
        ApprovalNodeType nodeType,
        string? approverRole = null,
        Guid? approverUserId = null,
        string? conditionExpression = null,
        int order = 0,
        bool isRequired = true)
    {
        if (order <= 0)
            order = Nodes.Count + 1;

        var node = new ApprovalNode(
            Guid.NewGuid(),
            Id,
            nodeName,
            nodeType,
            approverRole,
            approverUserId,
            conditionExpression,
            order,
            isRequired);
        Nodes.Add(node);
        return node;
    }

    /// <summary>Remove an approval node from the flow.</summary>
    public void RemoveNode(Guid nodeId)
    {
        var node = Nodes.FirstOrDefault(n => n.Id == nodeId);
        if (node == null)
            throw new BusinessException("WMS:Workflow:0101", "Approval node not found.");
        Nodes.Remove(node);
    }

    /// <summary>Update an existing approval node.</summary>
    public void UpdateNode(
        Guid nodeId,
        string nodeName,
        ApprovalNodeType nodeType,
        string? approverRole = null,
        Guid? approverUserId = null,
        string? conditionExpression = null,
        int order = 0,
        bool isRequired = true)
    {
        var node = Nodes.FirstOrDefault(n => n.Id == nodeId);
        if (node == null)
            throw new BusinessException("WMS:Workflow:0101", "Approval node not found.");

        node.Update(nodeName, nodeType, approverRole, approverUserId, conditionExpression, order, isRequired);
    }

    /// <summary>Activate the flow.</summary>
    public void Activate()
    {
        if (Nodes.Count == 0)
            throw new BusinessException("WMS:Workflow:0102", "Cannot activate flow with no nodes.");
        IsActive = true;
    }

    /// <summary>Deactivate the flow.</summary>
    public void Deactivate()
    {
        IsActive = false;
    }
}

/// <summary>
/// ApprovalNode — sub-entity of ApprovalFlow (AGG-24).
/// Represents a single node in the approval flow template.
/// </summary>
public class ApprovalNode : Entity<Guid>
{
    // ── Properties ──────────────────────────────────────────────
    public Guid FlowId { get; private set; }
    public string NodeName { get; private set; }
    public ApprovalNodeType NodeType { get; private set; }
    public string? ApproverRole { get; private set; }
    public Guid? ApproverUserId { get; private set; }
    public string? ConditionExpression { get; private set; }
    public int Order { get; private set; }
    public bool IsRequired { get; private set; }

    // ── Constructors ────────────────────────────────────────────
    protected ApprovalNode() { } // EF Core

    public ApprovalNode(
        Guid id,
        Guid flowId,
        string nodeName,
        ApprovalNodeType nodeType,
        string? approverRole = null,
        Guid? approverUserId = null,
        string? conditionExpression = null,
        int order = 0,
        bool isRequired = true)
    {
        Id = id;
        FlowId = flowId;
        NodeName = nodeName ?? throw new ArgumentNullException(nameof(nodeName));
        NodeType = nodeType ?? throw new ArgumentNullException(nameof(nodeType));
        ApproverRole = approverRole;
        ApproverUserId = approverUserId;
        ConditionExpression = conditionExpression;
        Order = order;
        IsRequired = isRequired;
    }

    // ── Methods ─────────────────────────────────────────────────
    internal void Update(
        string nodeName,
        ApprovalNodeType nodeType,
        string? approverRole,
        Guid? approverUserId,
        string? conditionExpression,
        int order,
        bool isRequired)
    {
        NodeName = nodeName ?? throw new ArgumentNullException(nameof(nodeName));
        NodeType = nodeType ?? throw new ArgumentNullException(nameof(nodeType));
        ApproverRole = approverRole;
        ApproverUserId = approverUserId;
        ConditionExpression = conditionExpression;
        Order = order;
        IsRequired = isRequired;
    }
}
