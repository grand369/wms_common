using Shouldly;
using Wms.Inbound.Domain.Aggregates;
using Wms.Inbound.Domain.Enums;
using Wms.Shared.Domain.Enums;
using Xunit;

namespace Wms.Inbound.Tests.Domain;

/// <summary>
/// InboundOrder Domain Tests — covers creation, line management, state transitions,
/// over-receipt validation, and cancellation.
/// </summary>
public class InboundOrderTests
{
    private InboundOrder CreateTestOrder()
    {
        return new InboundOrder(
            Guid.NewGuid(),
            InboundType.PurchaseReceipt,
            Guid.NewGuid(), "WH-001",
            0.05m, true,
            Guid.NewGuid(), "PO-001",
            null, null,
            Guid.NewGuid(), "Supplier-A");
    }

    [Fact]
    public void Create_InboundOrder_ShouldHaveDraftStatus()
    {
        var order = CreateTestOrder();

        order.InboundOrderNo.ShouldNotBeNullOrEmpty();
        order.InboundType.ShouldBe(InboundType.PurchaseReceipt);
        order.InboundStatus.ShouldBe(InboundStatus.Draft);
        order.WarehouseCode.ShouldBe("WH-001");
        order.OverReceiptRatio.ShouldBe(0.05m);
        order.QualityInspectionRequired.ShouldBeTrue();
        order.TotalPlanQuantity.ShouldBe(0m);
        order.Lines.ShouldBeEmpty();
    }

    [Fact]
    public void Create_InboundOrder_WithoutPurchaseOrder_ShouldThrow()
    {
        Should.Throw<BusinessException>(() =>
        {
            new InboundOrder(
                Guid.NewGuid(),
                InboundType.PurchaseReceipt,
                Guid.NewGuid(), "WH-001",
                0m, true,
                null, null, null, null, null, null);
        });
    }

    [Fact]
    public void AddLine_ShouldIncreaseTotalPlanQuantity()
    {
        var order = CreateTestOrder();

        order.AddLine(Guid.NewGuid(), 1, Guid.NewGuid(), "MAT-001", "Material-A", 100m);
        order.AddLine(Guid.NewGuid(), 2, Guid.NewGuid(), "MAT-002", "Material-B", 50m);

        order.Lines.Count.ShouldBe(2);
        order.TotalPlanQuantity.ShouldBe(150m);
    }

    [Fact]
    public void AddLine_WhenNotDraft_ShouldThrow()
    {
        var order = CreateTestOrder();
        order.AddLine(Guid.NewGuid(), 1, Guid.NewGuid(), "MAT-001", "Material-A", 100m);
        order.ConfirmReceipt(); // Would fail if received qty = 0 — need to set received first

        // Actually we need to receive quantity first
        var order2 = CreateTestOrder();
        var lineId = Guid.NewGuid();
        order2.AddLine(lineId, 1, Guid.NewGuid(), "MAT-001", "Material-A", 100m);
        order2.ReceiveLineQuantity(lineId, 100m);
        order2.ConfirmReceipt();

        Should.Throw<BusinessException>(() =>
        {
            order2.AddLine(Guid.NewGuid(), 2, Guid.NewGuid(), "MAT-002", "Material-B", 50m);
        });
    }

    [Fact]
    public void ConfirmReceipt_ShouldTransitionToConfirmed()
    {
        var order = CreateTestOrder();
        var lineId = Guid.NewGuid();
        order.AddLine(lineId, 1, Guid.NewGuid(), "MAT-001", "Material-A", 100m);
        order.ReceiveLineQuantity(lineId, 100m);

        order.ConfirmReceipt();

        order.InboundStatus.ShouldBe(InboundStatus.Confirmed);
        order.TotalReceivedQuantity.ShouldBe(100m);
    }

    [Fact]
    public void ConfirmReceipt_OverReceiptExceeded_ShouldThrow()
    {
        var order = new InboundOrder(
            Guid.NewGuid(),
            InboundType.PurchaseReceipt,
            Guid.NewGuid(), "WH-001",
            0.05m, true,
            Guid.NewGuid(), "PO-001",
            null, null,
            Guid.NewGuid(), "Supplier-A");

        var lineId = Guid.NewGuid();
        order.AddLine(lineId, 1, Guid.NewGuid(), "MAT-001", "Material-A", 100m);
        order.ReceiveLineQuantity(lineId, 110m); // 110 > 100 * (1 + 0.05) = 105

        Should.Throw<BusinessException>(() =>
        {
            order.ConfirmReceipt();
        });
    }

    [Fact]
    public void StartQualityInspection_ShouldTransitionToInspecting()
    {
        var order = CreateTestOrder();
        var lineId = Guid.NewGuid();
        order.AddLine(lineId, 1, Guid.NewGuid(), "MAT-001", "Material-A", 100m);
        order.ReceiveLineQuantity(lineId, 100m);
        order.ConfirmReceipt();

        order.StartQualityInspection();

        order.InboundStatus.ShouldBe(InboundStatus.Inspecting);
    }

    [Fact]
    public void StartQualityInspection_SkipQuality_ShouldTransitionToPutaway()
    {
        var order = new InboundOrder(
            Guid.NewGuid(),
            InboundType.PurchaseReceipt,
            Guid.NewGuid(), "WH-001",
            0m, false, // No quality inspection required
            Guid.NewGuid(), "PO-001",
            null, null,
            Guid.NewGuid(), "Supplier-A");

        var lineId = Guid.NewGuid();
        order.AddLine(lineId, 1, Guid.NewGuid(), "MAT-001", "Material-A", 100m);
        order.ReceiveLineQuantity(lineId, 100m);
        order.ConfirmReceipt();

        order.StartQualityInspection(); // Should skip and go to Putaway

        order.InboundStatus.ShouldBe(InboundStatus.Putaway);
        order.Lines.First().QualityStatus.ShouldBe(QualityStatus.Skip);
    }

    [Fact]
    public void QualityPass_ShouldSetLineQualified()
    {
        var order = CreateTestOrder();
        var lineId = Guid.NewGuid();
        order.AddLine(lineId, 1, Guid.NewGuid(), "MAT-001", "Material-A", 100m);
        order.ReceiveLineQuantity(lineId, 100m);
        order.ConfirmReceipt();
        order.StartQualityInspection();

        order.QualityPass(lineId);

        order.Lines.First().QualityStatus.ShouldBe(QualityStatus.Qualified);
        order.InboundStatus.ShouldBe(InboundStatus.Putaway); // All lines passed
    }

    [Fact]
    public void QualityFail_ShouldSetLineUnqualifiedAndOrderIsolated()
    {
        var order = CreateTestOrder();
        var lineId = Guid.NewGuid();
        order.AddLine(lineId, 1, Guid.NewGuid(), "MAT-001", "Material-A", 100m);
        order.ReceiveLineQuantity(lineId, 100m);
        order.ConfirmReceipt();
        order.StartQualityInspection();

        order.QualityFail(lineId);

        order.Lines.First().QualityStatus.ShouldBe(QualityStatus.Unqualified);
        order.InboundStatus.ShouldBe(InboundStatus.Isolated);
    }

    [Fact]
    public void ConfirmPutaway_ShouldSetPutawayLocation()
    {
        var order = new InboundOrder(
            Guid.NewGuid(),
            InboundType.PurchaseReceipt,
            Guid.NewGuid(), "WH-001",
            0m, false, // Skip quality inspection
            Guid.NewGuid(), "PO-001",
            null, null,
            Guid.NewGuid(), "Supplier-A");

        var lineId = Guid.NewGuid();
        order.AddLine(lineId, 1, Guid.NewGuid(), "MAT-001", "Material-A", 100m);
        order.ReceiveLineQuantity(lineId, 100m);
        order.ConfirmReceipt();
        order.StartQualityInspection(); // Transitions to Putaway (skip inspection)

        var locationId = Guid.NewGuid();
        order.ConfirmPutaway(lineId, locationId, "LOC-001", 100m);

        order.Lines.First().PutawayLocationId.ShouldBe(locationId);
        order.Lines.First().PutawayLocationCode.ShouldBe("LOC-001");
    }

    [Fact]
    public void ConfirmPutaway_UnqualifiedLine_ShouldThrow()
    {
        var order = CreateTestOrder();
        var lineId = Guid.NewGuid();
        order.AddLine(lineId, 1, Guid.NewGuid(), "MAT-001", "Material-A", 100m);
        order.ReceiveLineQuantity(lineId, 100m);
        order.ConfirmReceipt();
        order.StartQualityInspection();
        order.QualityFail(lineId); // Line is now Unqualified, order is Isolated

        Should.Throw<BusinessException>(() =>
        {
            order.ConfirmPutaway(lineId, Guid.NewGuid(), "LOC-001", 100m);
        });
    }

    [Fact]
    public void Complete_ShouldTransitionToCompleted()
    {
        var order = new InboundOrder(
            Guid.NewGuid(),
            InboundType.PurchaseReceipt,
            Guid.NewGuid(), "WH-001",
            0m, false,
            Guid.NewGuid(), "PO-001",
            null, null,
            Guid.NewGuid(), "Supplier-A");

        var lineId = Guid.NewGuid();
        order.AddLine(lineId, 1, Guid.NewGuid(), "MAT-001", "Material-A", 100m);
        order.ReceiveLineQuantity(lineId, 100m);
        order.ConfirmReceipt();
        order.StartQualityInspection();
        order.ConfirmPutaway(lineId, Guid.NewGuid(), "LOC-001", 100m);

        order.Complete();

        order.InboundStatus.ShouldBe(InboundStatus.Completed);
        order.IsCompleted.ShouldBeTrue();
        order.CompletionTime.ShouldNotBeNull();
    }

    [Fact]
    public void Cancel_InDraft_ShouldTransitionToCancelled()
    {
        var order = CreateTestOrder();
        order.Cancel();

        order.InboundStatus.ShouldBe(InboundStatus.Cancelled);
    }

    [Fact]
    public void Cancel_InCompleted_ShouldThrow()
    {
        var order = new InboundOrder(
            Guid.NewGuid(),
            InboundType.PurchaseReceipt,
            Guid.NewGuid(), "WH-001",
            0m, false,
            Guid.NewGuid(), "PO-001",
            null, null,
            Guid.NewGuid(), "Supplier-A");

        var lineId = Guid.NewGuid();
        order.AddLine(lineId, 1, Guid.NewGuid(), "MAT-001", "Material-A", 100m);
        order.ReceiveLineQuantity(lineId, 100m);
        order.ConfirmReceipt();
        order.StartQualityInspection();
        order.ConfirmPutaway(lineId, Guid.NewGuid(), "LOC-001", 100m);
        order.Complete();

        Should.Throw<BusinessException>(() =>
        {
            order.Cancel();
        });
    }

    [Fact]
    public void RemoveLine_ShouldDecreaseTotalPlanQuantity()
    {
        var order = CreateTestOrder();
        var lineId1 = Guid.NewGuid();
        var lineId2 = Guid.NewGuid();
        order.AddLine(lineId1, 1, Guid.NewGuid(), "MAT-001", "Material-A", 100m);
        order.AddLine(lineId2, 2, Guid.NewGuid(), "MAT-002", "Material-B", 50m);

        order.RemoveLine(lineId2);

        order.Lines.Count.ShouldBe(1);
        order.TotalPlanQuantity.ShouldBe(100m);
    }
}
