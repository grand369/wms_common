using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Shouldly;
using Volo.Abp;
using Volo.Abp.Testing;
using Wms.Shared.Domain.Enums;
using Wms.Transfer.Domain.Aggregates;
using Wms.Transfer.Domain.Enums;

namespace Wms.Transfer.Tests.Domain;

/// <summary>
/// TransferOrder domain tests — SM-05 state machine + line management + BR-033 validation
/// </summary>
public class TransferOrderTests : AbpIntegratedTest<WmsTransferTestModule>
{
    private TransferOrder CreateSampleOrder()
    {
        var order = new TransferOrder(
            Guid.NewGuid(),
            "TF-2026-001",
            TransferType.WarehouseTransfer,
            Guid.NewGuid(), "WH-A",
            Guid.NewGuid(), "WH-B",
            false);

        order.AddLine(Guid.NewGuid(), "MAT-001", 100);
        order.AddLine(Guid.NewGuid(), "MAT-002", 50);
        return order;
    }

    // ── SM-05 State Machine Tests ─────────────────────────

    [Fact]
    public void Draft_Order_Can_Submit_Approval()
    {
        var order = CreateSampleOrder();
        order.SubmitApproval();
        order.TransferStatus.ShouldBe(TransferStatus.Approved);
        order.ApprovalStatus.ShouldBe(ApprovalStatus.Pending);
    }

    [Fact]
    public void Approved_Order_Can_Be_Approved()
    {
        var order = CreateSampleOrder();
        order.SubmitApproval();
        order.Approve();
        order.ApprovalStatus.ShouldBe(ApprovalStatus.Approved);
    }

    [Fact]
    public void Approved_Order_Can_Confirm_Outbound()
    {
        var order = CreateSampleOrder();
        order.SubmitApproval();
        order.Approve();
        order.ConfirmOutbound();
        order.TransferStatus.ShouldBe(TransferStatus.InTransit);
    }

    [Fact]
    public void InTransit_Order_Can_Confirm_Inbound()
    {
        var order = CreateSampleOrder();
        order.SubmitApproval();
        order.Approve();
        order.ConfirmOutbound();
        order.ConfirmInbound();
        order.TransferStatus.ShouldBe(TransferStatus.Received);
    }

    [Fact]
    public void Received_Order_Can_Complete()
    {
        var order = CreateSampleOrder();
        order.SubmitApproval();
        order.Approve();
        order.ConfirmOutbound();
        order.ConfirmInbound();
        order.Complete();
        order.TransferStatus.ShouldBe(TransferStatus.Completed);
    }

    [Fact]
    public void Completed_Order_Can_Close()
    {
        var order = CreateSampleOrder();
        order.SubmitApproval();
        order.Approve();
        order.ConfirmOutbound();
        order.ConfirmInbound();
        order.Complete();
        order.Close();
        order.TransferStatus.ShouldBe(TransferStatus.Closed);
    }

    [Fact]
    public void Draft_Order_Can_Cancel()
    {
        var order = CreateSampleOrder();
        order.Cancel("no longer needed");
        order.TransferStatus.ShouldBe(TransferStatus.Cancelled);
    }

    [Fact]
    public void Pending_Approval_Can_Be_Rejected()
    {
        var order = CreateSampleOrder();
        order.SubmitApproval();
        order.Reject("insufficient stock");
        order.TransferStatus.ShouldBe(TransferStatus.Rejected);
        order.ApprovalStatus.ShouldBe(ApprovalStatus.Rejected);
    }

    // ── Invalid Transition Tests ──────────────────────────

    [Fact]
    public void NonDraft_Cannot_Submit_Approval()
    {
        var order = CreateSampleOrder();
        order.SubmitApproval();
        Should.Throw<BusinessException>(() => order.SubmitApproval());
    }

    [Fact]
    public void InTransit_Cannot_Complete()
    {
        var order = CreateSampleOrder();
        order.SubmitApproval();
        order.Approve();
        order.ConfirmOutbound();
        Should.Throw<BusinessException>(() => order.Complete());
    }

    // ── Line Management Tests ─────────────────────────────

    [Fact]
    public void AddLine_Increases_LineCount()
    {
        var order = CreateSampleOrder();
        order.Lines.Count.ShouldBe(2);
    }

    [Fact]
    public void OutboundConfirmed_Cannot_Exceed_TransferQuantity()
    {
        var order = CreateSampleOrder();
        Should.Throw<BusinessException>(() => order.UpdateOutboundConfirmedQuantity(1, 200));
    }

    [Fact]
    public void InboundConfirmed_Cannot_Exceed_OutboundConfirmed()
    {
        var order = CreateSampleOrder();
        order.UpdateOutboundConfirmedQuantity(1, 80);
        Should.Throw<BusinessException>(() => order.UpdateInboundConfirmedQuantity(1, 90));
    }
}
