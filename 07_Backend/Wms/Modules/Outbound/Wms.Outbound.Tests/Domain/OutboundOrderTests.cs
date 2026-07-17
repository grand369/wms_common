using Shouldly;
using Wms.Outbound.Domain.Aggregates;
using Wms.Outbound.Domain.Enums;
using Wms.Shared.Domain.Enums;
using Xunit;

namespace Wms.Outbound.Tests.Domain;

/// <summary>
/// OutboundOrder Domain Tests — covers creation, line management, state transitions (SM-02),
/// allocation, picking, shipping, completion, cancellation, and release allocation.
/// </summary>
public class OutboundOrderTests
{
    private OutboundOrder CreateTestOrder()
    {
        return new OutboundOrder(
            Guid.NewGuid(),
            OutboundType.MaterialRequisition,
            Guid.NewGuid(), "WH-001",
            0.05m, false,
            Guid.NewGuid(), null, null);
    }

    [Fact]
    public void Create_OutboundOrder_ShouldHaveDraftStatus()
    {
        var order = CreateTestOrder();

        order.OutboundOrderNo.ShouldNotBeNullOrEmpty();
        order.OutboundType.ShouldBe(OutboundType.MaterialRequisition);
        order.OutboundStatus.ShouldBe(OutboundStatus.Draft);
        order.WarehouseCode.ShouldBe("WH-001");
        order.OverIssueRatio.ShouldBe(0.05m);
        order.IsEmergency.ShouldBeFalse();
        order.TotalRequiredQuantity.ShouldBe(0m);
        order.Lines.ShouldBeEmpty();
    }

    [Fact]
    public void Create_OutboundOrder_WithoutMaterialRequisition_ShouldThrow()
    {
        Should.Throw<BusinessException>(() =>
        {
            new OutboundOrder(
                Guid.NewGuid(),
                OutboundType.MaterialRequisition,
                Guid.NewGuid(), "WH-001",
                0m, false,
                null, null, null);
        });
    }

    [Fact]
    public void Create_OutboundOrder_SalesShipment_WithoutSalesOrder_ShouldThrow()
    {
        Should.Throw<BusinessException>(() =>
        {
            new OutboundOrder(
                Guid.NewGuid(),
                OutboundType.SalesShipment,
                Guid.NewGuid(), "WH-001",
                0m, false,
                null, null, null);
        });
    }

    [Fact]
    public void AddLine_ShouldIncreaseTotalRequiredQuantity()
    {
        var order = CreateTestOrder();

        order.AddLine(Guid.NewGuid(), 1, Guid.NewGuid(), "MAT-001", "Material-A", 100m);
        order.AddLine(Guid.NewGuid(), 2, Guid.NewGuid(), "MAT-002", "Material-B", 50m);

        order.Lines.Count.ShouldBe(2);
        order.TotalRequiredQuantity.ShouldBe(150m);
    }

    [Fact]
    public void AddLine_WithIssueStrategy_ShouldSetStrategy()
    {
        var order = CreateTestOrder();

        order.AddLine(Guid.NewGuid(), 1, Guid.NewGuid(), "MAT-001", "Material-A", 100m,
            issueStrategyValue: 1); // FEFO

        order.Lines.First().IssueStrategy.ShouldBe(IssueStrategyType.FEFO);
    }

    [Fact]
    public void AddLine_WhenNotDraft_ShouldThrow()
    {
        var order = CreateTestOrder();
        var lineId = Guid.NewGuid();
        order.AddLine(lineId, 1, Guid.NewGuid(), "MAT-001", "Material-A", 100m);

        // Allocate to transition out of Draft
        var locationId = Guid.NewGuid();
        order.Allocate(new List<(Guid, decimal, Guid?, string?)>
        {
            (lineId, 100m, locationId, "LOC-001")
        });

        Should.Throw<BusinessException>(() =>
        {
            order.AddLine(Guid.NewGuid(), 2, Guid.NewGuid(), "MAT-002", "Material-B", 50m);
        });
    }

    [Fact]
    public void RemoveLine_ShouldDecreaseTotalRequiredQuantity()
    {
        var order = CreateTestOrder();
        var lineId1 = Guid.NewGuid();
        var lineId2 = Guid.NewGuid();
        order.AddLine(lineId1, 1, Guid.NewGuid(), "MAT-001", "Material-A", 100m);
        order.AddLine(lineId2, 2, Guid.NewGuid(), "MAT-002", "Material-B", 50m);

        order.RemoveLine(lineId2);

        order.Lines.Count.ShouldBe(1);
        order.TotalRequiredQuantity.ShouldBe(100m);
    }

    [Fact]
    public void Allocate_ShouldTransitionToAllocated()
    {
        var order = CreateTestOrder();
        var lineId = Guid.NewGuid();
        order.AddLine(lineId, 1, Guid.NewGuid(), "MAT-001", "Material-A", 100m);

        var locationId = Guid.NewGuid();
        order.Allocate(new List<(Guid, decimal, Guid?, string?)>
        {
            (lineId, 100m, locationId, "LOC-001")
        });

        order.OutboundStatus.ShouldBe(OutboundStatus.Allocated);
        order.TotalAllocatedQuantity.ShouldBe(100m);
        order.Lines.First().AllocatedQuantity.ShouldBe(100m);
        order.Lines.First().PickingLocationId.ShouldBe(locationId);
    }

    [Fact]
    public void Allocate_OverIssueExceeded_ShouldThrow()
    {
        var order = new OutboundOrder(
            Guid.NewGuid(),
            OutboundType.MaterialRequisition,
            Guid.NewGuid(), "WH-001",
            0.05m, false, // OverIssueRatio = 5%
            Guid.NewGuid(), null, null);

        var lineId = Guid.NewGuid();
        order.AddLine(lineId, 1, Guid.NewGuid(), "MAT-001", "Material-A", 100m);

        // 110 > 100 * (1 + 0.05) = 105
        Should.Throw<BusinessException>(() =>
        {
            order.Allocate(new List<(Guid, decimal, Guid?, string?)>
            {
                (lineId, 110m, Guid.NewGuid(), "LOC-001")
            });
        });
    }

    [Fact]
    public void Allocate_WhenNotDraft_ShouldThrow()
    {
        var order = CreateTestOrder();
        var lineId = Guid.NewGuid();
        order.AddLine(lineId, 1, Guid.NewGuid(), "MAT-001", "Material-A", 100m);

        order.Allocate(new List<(Guid, decimal, Guid?, string?)>
        {
            (lineId, 100m, Guid.NewGuid(), "LOC-001")
        });

        // Already Allocated — cannot allocate again
        Should.Throw<BusinessException>(() =>
        {
            order.Allocate(new List<(Guid, decimal, Guid?, string?)>
            {
                (lineId, 50m, Guid.NewGuid(), "LOC-002")
            });
        });
    }

    [Fact]
    public void ConfirmPicking_ShouldTransitionToPicking()
    {
        var order = CreateTestOrder();
        var lineId = Guid.NewGuid();
        order.AddLine(lineId, 1, Guid.NewGuid(), "MAT-001", "Material-A", 100m);
        order.Allocate(new List<(Guid, decimal, Guid?, string?)>
        {
            (lineId, 100m, Guid.NewGuid(), "LOC-001")
        });

        order.ConfirmPicking(new List<(Guid, decimal)>
        {
            (lineId, 100m)
        });

        order.OutboundStatus.ShouldBe(OutboundStatus.Picking);
        order.TotalPickedQuantity.ShouldBe(100m);
        order.Lines.First().PickedQuantity.ShouldBe(100m);
    }

    [Fact]
    public void ConfirmPicking_WhenNotAllocated_ShouldThrow()
    {
        var order = CreateTestOrder();
        var lineId = Guid.NewGuid();
        order.AddLine(lineId, 1, Guid.NewGuid(), "MAT-001", "Material-A", 100m);

        Should.Throw<BusinessException>(() =>
        {
            order.ConfirmPicking(new List<(Guid, decimal)>
            {
                (lineId, 100m)
            });
        });
    }

    [Fact]
    public void ConfirmShipping_ShouldTransitionToShipped()
    {
        var order = CreateTestOrder();
        var lineId = Guid.NewGuid();
        order.AddLine(lineId, 1, Guid.NewGuid(), "MAT-001", "Material-A", 100m);
        order.Allocate(new List<(Guid, decimal, Guid?, string?)>
        {
            (lineId, 100m, Guid.NewGuid(), "LOC-001")
        });
        order.ConfirmPicking(new List<(Guid, decimal)>
        {
            (lineId, 100m)
        });

        order.ConfirmShipping(new List<(Guid, decimal)>
        {
            (lineId, 95m) // Shipped less than picked
        });

        order.OutboundStatus.ShouldBe(OutboundStatus.Shipped);
        order.TotalShippedQuantity.ShouldBe(95m);
        order.Lines.First().ShippedQuantity.ShouldBe(95m);
    }

    [Fact]
    public void ConfirmShipping_ShippedExceedsPicked_ShouldThrow()
    {
        var order = CreateTestOrder();
        var lineId = Guid.NewGuid();
        order.AddLine(lineId, 1, Guid.NewGuid(), "MAT-001", "Material-A", 100m);
        order.Allocate(new List<(Guid, decimal, Guid?, string?)>
        {
            (lineId, 100m, Guid.NewGuid(), "LOC-001")
        });
        order.ConfirmPicking(new List<(Guid, decimal)>
        {
            (lineId, 80m)
        });

        // 90 > 80 (shipped > picked) — OB-006
        Should.Throw<BusinessException>(() =>
        {
            order.ConfirmShipping(new List<(Guid, decimal)>
            {
                (lineId, 90m)
            });
        });
    }

    [Fact]
    public void Complete_ShouldTransitionToCompleted()
    {
        var order = CreateTestOrder();
        var lineId = Guid.NewGuid();
        order.AddLine(lineId, 1, Guid.NewGuid(), "MAT-001", "Material-A", 100m);
        order.Allocate(new List<(Guid, decimal, Guid?, string?)>
        {
            (lineId, 100m, Guid.NewGuid(), "LOC-001")
        });
        order.ConfirmPicking(new List<(Guid, decimal)>
        {
            (lineId, 100m)
        });
        order.ConfirmShipping(new List<(Guid, decimal)>
        {
            (lineId, 100m)
        });

        order.Complete();

        order.OutboundStatus.ShouldBe(OutboundStatus.Completed);
        order.IsCompleted.ShouldBeTrue();
        order.CompletionTime.ShouldNotBeNull();
    }

    [Fact]
    public void Complete_WhenNotShipped_ShouldThrow()
    {
        var order = CreateTestOrder();
        var lineId = Guid.NewGuid();
        order.AddLine(lineId, 1, Guid.NewGuid(), "MAT-001", "Material-A", 100m);
        order.Allocate(new List<(Guid, decimal, Guid?, string?)>
        {
            (lineId, 100m, Guid.NewGuid(), "LOC-001")
        });

        Should.Throw<BusinessException>(() =>
        {
            order.Complete();
        });
    }

    [Fact]
    public void Cancel_InDraft_ShouldTransitionToCancelled()
    {
        var order = CreateTestOrder();
        order.Cancel();

        order.OutboundStatus.ShouldBe(OutboundStatus.Cancelled);
    }

    [Fact]
    public void Cancel_InAllocated_ShouldThrow()
    {
        var order = CreateTestOrder();
        var lineId = Guid.NewGuid();
        order.AddLine(lineId, 1, Guid.NewGuid(), "MAT-001", "Material-A", 100m);
        order.Allocate(new List<(Guid, decimal, Guid?, string?)>
        {
            (lineId, 100m, Guid.NewGuid(), "LOC-001")
        });

        // Cannot cancel from Allocated — must release allocation first
        Should.Throw<BusinessException>(() =>
        {
            order.Cancel();
        });
    }

    [Fact]
    public void ReleaseAllocation_ShouldTransitionBackToDraft()
    {
        var order = CreateTestOrder();
        var lineId = Guid.NewGuid();
        order.AddLine(lineId, 1, Guid.NewGuid(), "MAT-001", "Material-A", 100m);
        order.Allocate(new List<(Guid, decimal, Guid?, string?)>
        {
            (lineId, 100m, Guid.NewGuid(), "LOC-001")
        });

        order.ReleaseAllocation();

        order.OutboundStatus.ShouldBe(OutboundStatus.Draft);
        order.TotalAllocatedQuantity.ShouldBe(0m);
        order.Lines.First().AllocatedQuantity.ShouldBe(0m);
    }

    [Fact]
    public void ReleaseAllocation_WhenNotAllocated_ShouldThrow()
    {
        var order = CreateTestOrder();

        Should.Throw<BusinessException>(() =>
        {
            order.ReleaseAllocation();
        });
    }

    [Fact]
    public void FullLifecycle_Draft_To_Completed()
    {
        var order = CreateTestOrder();
        var lineId = Guid.NewGuid();
        order.AddLine(lineId, 1, Guid.NewGuid(), "MAT-001", "Material-A", 100m);

        // Draft → Allocated
        order.Allocate(new List<(Guid, decimal, Guid?, string?)>
        {
            (lineId, 100m, Guid.NewGuid(), "LOC-001")
        });
        order.OutboundStatus.ShouldBe(OutboundStatus.Allocated);

        // Allocated → Picking
        order.ConfirmPicking(new List<(Guid, decimal)>
        {
            (lineId, 100m)
        });
        order.OutboundStatus.ShouldBe(OutboundStatus.Picking);

        // Picking → Shipped
        order.ConfirmShipping(new List<(Guid, decimal)>
        {
            (lineId, 100m)
        });
        order.OutboundStatus.ShouldBe(OutboundStatus.Shipped);

        // Shipped → Completed
        order.Complete();
        order.OutboundStatus.ShouldBe(OutboundStatus.Completed);
        order.IsCompleted.ShouldBeTrue();
    }
}
