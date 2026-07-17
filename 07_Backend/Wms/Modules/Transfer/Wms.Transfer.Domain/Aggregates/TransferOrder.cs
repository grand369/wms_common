using System;
using System.Collections.Generic;
using Wms.Shared.Domain.Enums;
using Wms.Transfer.Domain.Enums;
using Wms.Transfer.Domain.Events;

namespace Wms.Transfer.Domain.Aggregates;

/// <summary>
/// TransferOrder Aggregate Root — AGG-15
/// Manages transfer operations: CrossArea, CrossWarehouse, CrossFactory.
/// SM-05 state machine: Draft → Approved → InTransit → Received → Completed → Closed
/// + Rejected / Cancelled branches
/// </summary>
public class TransferOrder : FullAuditedAggregateRoot<Guid>
{
    // ── Properties ──────────────────────────────────────────────
    public string TransferOrderNo { get; private set; }
    public TransferType TransferType { get; private set; }
    public TransferStatus TransferStatus { get; private set; }
    public Guid SourceWarehouseId { get; private set; }
    public string SourceWarehouseCode { get; private set; }
    public Guid TargetWarehouseId { get; private set; }
    public string TargetWarehouseCode { get; private set; }
    public ApprovalStatus ApprovalStatus { get; private set; }
    public bool IsCrossCompany { get; private set; }
    public string? Remark { get; private set; }

    // ── Navigation ──────────────────────────────────────────────
    public List<TransferLine> Lines { get; private set; } = new();

    // ── Constructors ────────────────────────────────────────────
    protected TransferOrder() { } // EF

    public TransferOrder(
        Guid id,
        string transferOrderNo,
        TransferType transferType,
        Guid sourceWarehouseId,
        string sourceWarehouseCode,
        Guid targetWarehouseId,
        string targetWarehouseCode,
        bool isCrossCompany = false,
        string? remark = null)
    {
        Id = id;
        TransferOrderNo = transferOrderNo ?? throw new ArgumentNullException(nameof(transferOrderNo));
        TransferType = transferType ?? throw new ArgumentNullException(nameof(transferType));
        SourceWarehouseId = sourceWarehouseId;
        SourceWarehouseCode = sourceWarehouseCode ?? throw new ArgumentNullException(nameof(sourceWarehouseCode));
        TargetWarehouseId = targetWarehouseId;
        TargetWarehouseCode = targetWarehouseCode ?? throw new ArgumentNullException(nameof(targetWarehouseCode));
        TransferStatus = TransferStatus.Draft;
        ApprovalStatus = ApprovalStatus.None;
        IsCrossCompany = isCrossCompany;
        Remark = remark;

        AddLocalEvent(new TransferCreatedEvent(id, transferOrderNo, transferType, sourceWarehouseId, targetWarehouseId));
    }

    // ── SM-05 State Transitions ─────────────────────────────────

    /// <summary>Submit for approval → Draft → Pending</summary>
    public void SubmitApproval()
    {
        if (TransferStatus != TransferStatus.Draft)
            throw new BusinessException("Wms.Transfer:0101", "Only Draft orders can submit approval.");
        TransferStatus = TransferStatus.Approved;
        ApprovalStatus = ApprovalStatus.Pending;
    }

    /// <summary>Approve → Pending → Approved</summary>
    public void Approve()
    {
        if (ApprovalStatus != ApprovalStatus.Pending)
            throw new BusinessException("Wms.Transfer:0102", "Only Pending approval orders can be approved.");
        ApprovalStatus = ApprovalStatus.Approved;
        // TransferStatus stays Approved (SM-05 transition: Draft → Approved)
    }

    /// <summary>Reject → Pending → Rejected</summary>
    public void Reject(string? reason = null)
    {
        if (ApprovalStatus != ApprovalStatus.Pending)
            throw new BusinessException("Wms.Transfer:0103", "Only Pending approval orders can be rejected.");
        ApprovalStatus = ApprovalStatus.Rejected;
        TransferStatus = TransferStatus.Rejected;
        Remark = reason ?? Remark;
    }

    /// <summary>Confirm source outbound → Approved → InTransit (DE-021)</summary>
    public void ConfirmOutbound()
    {
        if (TransferStatus != TransferStatus.Approved)
            throw new BusinessException("Wms.Transfer:0104", "Only Approved orders can confirm outbound.");
        TransferStatus = TransferStatus.InTransit;
        AddLocalEvent(new TransferOutboundEvent(Id, SourceWarehouseId, Lines));
    }

    /// <summary>Confirm target inbound → InTransit → Received (DE-022)</summary>
    public void ConfirmInbound()
    {
        if (TransferStatus != TransferStatus.InTransit)
            throw new BusinessException("Wms.Transfer:0105", "Only InTransit orders can confirm inbound.");
        TransferStatus = TransferStatus.Received;
        AddLocalEvent(new TransferInboundEvent(Id, TargetWarehouseId, Lines));
    }

    /// <summary>Complete → Received → Completed</summary>
    public void Complete()
    {
        if (TransferStatus != TransferStatus.Received)
            throw new BusinessException("Wms.Transfer:0106", "Only Received orders can be completed.");
        TransferStatus = TransferStatus.Completed;
    }

    /// <summary>Close → Completed → Closed</summary>
    public void Close()
    {
        if (TransferStatus != TransferStatus.Completed)
            throw new BusinessException("Wms.Transfer:0107", "Only Completed orders can be closed.");
        TransferStatus = TransferStatus.Closed;
    }

    /// <summary>Cancel → Draft → Cancelled</summary>
    public void Cancel(string? reason = null)
    {
        if (TransferStatus != TransferStatus.Draft)
            throw new BusinessException("Wms.Transfer:0108", "Only Draft orders can be cancelled.");
        TransferStatus = TransferStatus.Cancelled;
        Remark = reason ?? Remark;
    }

    // ── Line Management ─────────────────────────────────────────

    public TransferLine AddLine(Guid materialId, string materialCode, decimal transferQuantity)
    {
        var line = new TransferLine(
            Guid.NewGuid(),
            Id,
            Lines.Count + 1,
            materialId,
            materialCode,
            transferQuantity);
        Lines.Add(line);
        return line;
    }

    public void UpdateOutboundConfirmedQuantity(int lineNo, decimal confirmedQty)
    {
        var line = Lines.Find(l => l.LineNo == lineNo);
        if (line == null) throw new BusinessException("Wms.Transfer:0201", $"Line {lineNo} not found.");
        line.SetOutboundConfirmedQuantity(confirmedQty);
    }

    public void UpdateInboundConfirmedQuantity(int lineNo, decimal confirmedQty)
    {
        var line = Lines.Find(l => l.LineNo == lineNo);
        if (line == null) throw new BusinessException("Wms.Transfer:0202", $"Line {lineNo} not found.");
        line.SetInboundConfirmedQuantity(confirmedQty);
    }

    // ── Timeout Check (ER-011) ──────────────────────────────────

    public void CheckInTransitTimeout(TimeSpan expectedDuration)
    {
        if (TransferStatus != TransferStatus.InTransit) return;
        if (CreationTime != null && DateTime.UtcNow - CreationTime > expectedDuration)
        {
            AddLocalEvent(new TransferInTransitTimeoutEvent(Id, SourceWarehouseId, TargetWarehouseId));
        }
    }
}
