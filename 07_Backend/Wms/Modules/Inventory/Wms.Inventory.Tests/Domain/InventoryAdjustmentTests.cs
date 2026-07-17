using Shouldly;
using Wms.Inventory.Domain.Aggregates;
using Wms.Inventory.Domain.Enums;
using Wms.Shared.Domain.Enums;
using Xunit;

namespace Wms.Inventory.Tests.Domain;

/// <summary>
/// Inventory Adjustment Tests — verifies state transitions (Draft → Submitted → Approved → Executed).
/// </summary>
public class InventoryAdjustmentTests
{
    private InventoryAdjustment CreateTestAdjustment()
    {
        var adjustment = new InventoryAdjustment(
            Guid.NewGuid(),
            "ADJ-001",
            AdjustmentType.Gain,
            "盘点盘盈",
            Guid.NewGuid(),
            "WH-001");

        adjustment.AddLine(new InventoryAdjustmentLine(
            Guid.NewGuid(),
            adjustment.Id,
            1,
            Guid.NewGuid(), "MAT-001", "物料A",
            10m,
            Guid.NewGuid(), "LOC-001",
            null,
            InventoryStatus.Available,
            InventoryStatus.Available,
            "盘盈"));

        return adjustment;
    }

    [Fact]
    public void Create_Adjustment_ShouldBeInDraftStatus()
    {
        var adjustment = CreateTestAdjustment();

        adjustment.ApprovalStatus.ShouldBe(AdjustmentApprovalStatus.Draft);
        adjustment.Lines.Count.ShouldBe(1);
        adjustment.IsCompleted.ShouldBeFalse();
    }

    [Fact]
    public void Submit_Adjustment_ShouldChangeToSubmitted()
    {
        var adjustment = CreateTestAdjustment();
        adjustment.Submit();

        adjustment.ApprovalStatus.ShouldBe(AdjustmentApprovalStatus.Submitted);
    }

    [Fact]
    public void Submit_AdjustmentWithoutLines_ShouldThrowException()
    {
        var adjustment = new InventoryAdjustment(
            Guid.NewGuid(), "ADJ-002", AdjustmentType.Loss, "盘亏",
            Guid.NewGuid(), "WH-001");

        Should.Throw<BusinessException>(() => adjustment.Submit());
    }

    [Fact]
    public void Approve_Adjustment_ShouldChangeToApproved()
    {
        var adjustment = CreateTestAdjustment();
        adjustment.Submit();
        adjustment.Approve();

        adjustment.ApprovalStatus.ShouldBe(AdjustmentApprovalStatus.Approved);
    }

    [Fact]
    public void Reject_Adjustment_ShouldChangeToRejected()
    {
        var adjustment = CreateTestAdjustment();
        adjustment.Submit();
        adjustment.Reject();

        adjustment.ApprovalStatus.ShouldBe(AdjustmentApprovalStatus.Rejected);
    }

    [Fact]
    public void Execute_Adjustment_ShouldMarkCompleted()
    {
        var adjustment = CreateTestAdjustment();
        adjustment.Submit();
        adjustment.Approve();
        adjustment.Execute();

        adjustment.ApprovalStatus.ShouldBe(AdjustmentApprovalStatus.Executed);
        adjustment.IsCompleted.ShouldBeTrue();
        adjustment.CompletionTime.ShouldNotBeNull();
    }

    [Fact]
    public void Cancel_Adjustment_ShouldChangeToCancelled()
    {
        var adjustment = CreateTestAdjustment();
        adjustment.Cancel();

        adjustment.ApprovalStatus.ShouldBe(AdjustmentApprovalStatus.Cancelled);
    }

    [Fact]
    public void Cancel_ExecutedAdjustment_ShouldThrowException()
    {
        var adjustment = CreateTestAdjustment();
        adjustment.Submit();
        adjustment.Approve();
        adjustment.Execute();

        Should.Throw<BusinessException>(() => adjustment.Cancel());
    }
}
