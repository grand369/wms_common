using System;
using Shouldly;
using Volo.Abp;
using Volo.Abp.Testing;
using Wms.Workflow.Domain.Aggregates;
using Wms.Workflow.Domain.Enums;

namespace Wms.Workflow.Tests.Domain;

/// <summary>
/// ApprovalInstance domain tests — state transitions (Approve/Reject/Resubmit/Cancel).
/// </summary>
public class ApprovalInstanceDomainTests : AbpIntegratedTest<WmsWorkflowTestModule>
{
    private ApprovalInstance CreatePendingInstance()
    {
        return new ApprovalInstance(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Test Flow",
            Guid.NewGuid(),
            "Inbound",
            "IN-2026-001",
            Guid.NewGuid(),
            "SubmitUser");
    }

    private ApprovalInstance CreateInProgressInstance()
    {
        var instance = CreatePendingInstance();
        // Simulate advancing to first approval node
        typeof(ApprovalInstance)
            .GetMethod("AdvanceToNode",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.Invoke(instance, new object[] { Guid.NewGuid(), "Manager Approval", Guid.NewGuid(), "Approver" });
        return instance;
    }

    // ── Approve Tests ──────────────────────────────────────

    [Fact]
    public void Pending_Instance_Can_Approve()
    {
        var instance = CreateInProgressInstance();
        instance.Approve(Guid.NewGuid(), "Approved - looks good");
        instance.ActionLogs.Count.ShouldBe(1);
        instance.ActionLogs[0].ActionType.ShouldBe(ApprovalActionType.Approve);
        instance.ActionLogs[0].Comment.ShouldBe("Approved - looks good");
    }

    [Fact]
    public void InProgress_Instance_Can_Approve()
    {
        var instance = CreateInProgressInstance();
        instance.InstanceStatus.ShouldBe(ApprovalInstanceStatus.InProgress);

        instance.Approve(Guid.NewGuid(), "Approved");
        instance.ActionLogs.Count.ShouldBe(1);
    }

    // ── Reject Tests ───────────────────────────────────────

    [Fact]
    public void Pending_Instance_Can_Reject()
    {
        var instance = CreateInProgressInstance();
        instance.Reject(Guid.NewGuid(), "Missing documentation");

        instance.InstanceStatus.ShouldBe(ApprovalInstanceStatus.Rejected);
        instance.CompletedTime.ShouldNotBeNull();
        instance.ActionLogs[0].ActionType.ShouldBe(ApprovalActionType.Reject);
    }

    [Fact]
    public void InProgress_Instance_Can_Reject()
    {
        var instance = CreateInProgressInstance();
        instance.Reject(Guid.NewGuid(), "Needs revision");

        instance.InstanceStatus.ShouldBe(ApprovalInstanceStatus.Rejected);
    }

    // ── Resubmit Tests ─────────────────────────────────────

    [Fact]
    public void Rejected_Instance_Can_Resubmit()
    {
        var instance = CreateInProgressInstance();
        instance.Reject(Guid.NewGuid(), "Rejected");
        instance.InstanceStatus.ShouldBe(ApprovalInstanceStatus.Rejected);

        instance.Resubmit("Fixed the issues");
        instance.InstanceStatus.ShouldBe(ApprovalInstanceStatus.Resubmitted);
        instance.ActionLogs.Count.ShouldBe(2); // Reject + Resubmit
    }

    [Fact]
    public void NonRejected_Instance_Cannot_Resubmit()
    {
        var instance = CreateInProgressInstance();
        Should.Throw<BusinessException>(() => instance.Resubmit("Not allowed"));
    }

    // ── Cancel Tests ───────────────────────────────────────

    [Fact]
    public void Pending_Instance_Can_Cancel()
    {
        var instance = CreatePendingInstance();
        instance.InstanceStatus.ShouldBe(ApprovalInstanceStatus.Pending);

        instance.Cancel();
        instance.InstanceStatus.ShouldBe(ApprovalInstanceStatus.Cancelled);
        instance.CompletedTime.ShouldNotBeNull();
    }

    [Fact]
    public void InProgress_Instance_Can_Cancel()
    {
        var instance = CreateInProgressInstance();
        instance.Cancel();

        instance.InstanceStatus.ShouldBe(ApprovalInstanceStatus.Cancelled);
    }

    [Fact]
    public void AlreadyApproved_Instance_Cannot_Cancel()
    {
        var instance = CreateInProgressInstance();
        // Manually set to approved via internal method
        typeof(ApprovalInstance)
            .GetMethod("CompleteApproval",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.Invoke(instance, null);

        instance.InstanceStatus.ShouldBe(ApprovalInstanceStatus.Approved);
        Should.Throw<BusinessException>(() => instance.Cancel());
    }

    [Fact]
    public void AlreadyCancelled_Instance_Cannot_Cancel()
    {
        var instance = CreatePendingInstance();
        instance.Cancel();
        instance.InstanceStatus.ShouldBe(ApprovalInstanceStatus.Cancelled);

        Should.Throw<BusinessException>(() => instance.Cancel());
    }
}
