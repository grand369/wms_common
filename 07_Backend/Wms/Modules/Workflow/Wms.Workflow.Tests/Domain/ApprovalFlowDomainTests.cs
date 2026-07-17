using System;
using System.Linq;
using Shouldly;
using Volo.Abp;
using Volo.Abp.Testing;
using Wms.Workflow.Domain.Aggregates;
using Wms.Workflow.Domain.Enums;

namespace Wms.Workflow.Tests.Domain;

/// <summary>
/// ApprovalFlow domain tests — node management + activate/deactivate.
/// </summary>
public class ApprovalFlowDomainTests : AbpIntegratedTest<WmsWorkflowTestModule>
{
    private ApprovalFlow CreateSampleFlow()
    {
        var flow = new ApprovalFlow(
            Guid.NewGuid(),
            "Test Inbound Flow",
            ApprovalFlowType.Inbound,
            "Test flow for inbound approval");

        flow.AddNode(
            "Start Node",
            ApprovalNodeType.Start,
            order: 1);

        flow.AddNode(
            "Manager Approval",
            ApprovalNodeType.Approval,
            approverRole: "Manager",
            order: 2,
            isRequired: true);

        flow.AddNode(
            "End Node",
            ApprovalNodeType.End,
            order: 3);

        return flow;
    }

    // ── Node Management Tests ──────────────────────────────

    [Fact]
    public void AddNode_Increases_Node_Count()
    {
        var flow = CreateSampleFlow();
        flow.Nodes.Count.ShouldBe(3);

        flow.AddNode(
            "Additional Approval",
            ApprovalNodeType.Approval,
            approverRole: "Director",
            order: 4);

        flow.Nodes.Count.ShouldBe(4);
    }

    [Fact]
    public void RemoveNode_Decreases_Node_Count()
    {
        var flow = CreateSampleFlow();
        var nodeToRemove = flow.Nodes.First(n => n.NodeName == "Manager Approval");

        flow.RemoveNode(nodeToRemove.Id);
        flow.Nodes.Count.ShouldBe(2);
        flow.Nodes.Any(n => n.NodeName == "Manager Approval").ShouldBeFalse();
    }

    [Fact]
    public void RemoveNode_NonExistent_Throws_Exception()
    {
        var flow = CreateSampleFlow();
        Should.Throw<BusinessException>(() => flow.RemoveNode(Guid.NewGuid()));
    }

    [Fact]
    public void UpdateNode_Changes_Node_Properties()
    {
        var flow = CreateSampleFlow();
        var node = flow.Nodes.First(n => n.NodeName == "Manager Approval");

        flow.UpdateNode(
            node.Id,
            "Senior Manager Approval",
            ApprovalNodeType.Approval,
            approverRole: "SeniorManager",
            order: 2,
            isRequired: true);

        var updated = flow.Nodes.First(n => n.Id == node.Id);
        updated.NodeName.ShouldBe("Senior Manager Approval");
        updated.ApproverRole.ShouldBe("SeniorManager");
    }

    // ── Activate / Deactivate Tests ────────────────────────

    [Fact]
    public void Activate_With_Nodes_Sets_IsActive_True()
    {
        var flow = CreateSampleFlow();
        flow.Deactivate();
        flow.IsActive.ShouldBeFalse();

        flow.Activate();
        flow.IsActive.ShouldBeTrue();
    }

    [Fact]
    public void Activate_Without_Nodes_Throws_Exception()
    {
        var flow = new ApprovalFlow(
            Guid.NewGuid(),
            "Empty Flow",
            ApprovalFlowType.Transfer);

        Should.Throw<BusinessException>(() => flow.Activate());
    }

    [Fact]
    public void Deactivate_Sets_IsActive_False()
    {
        var flow = CreateSampleFlow();
        flow.IsActive.ShouldBeTrue();

        flow.Deactivate();
        flow.IsActive.ShouldBeFalse();
    }
}
