using Volo.Abp.Domain.Entities.Auditing;
using Wms.Inventory.Domain.Enums;

namespace Wms.Inventory.Domain.Aggregates;

/// <summary>
/// Inventory Adjustment Aggregate Root (AGG-08) — represents a stock adjustment request
/// (gain/loss/scrap/revaluation). Must be approved before execution.
/// Contains child entity collection: Lines.
/// </summary>
public class InventoryAdjustment : FullAuditedAggregateRoot<Guid>
{
    /// <summary>Adjustment number — business natural key, auto-generated.</summary>
    public string AdjustmentNo { get; private set; }

    /// <summary>Adjustment type — Gain/Loss/Scrap/Revaluation.</summary>
    public AdjustmentType AdjustmentType { get; private set; }

    /// <summary>Adjustment reason — mandatory description of why the adjustment is needed.</summary>
    public string AdjustmentReason { get; private set; }

    /// <summary>Approval status — lifecycle state of the adjustment.</summary>
    public AdjustmentApprovalStatus ApprovalStatus { get; private set; }

    /// <summary>Warehouse ID — the warehouse where adjustment is performed.</summary>
    public Guid WarehouseId { get; private set; }

    /// <summary>Warehouse code — redundant field.</summary>
    public string WarehouseCode { get; private set; }

    /// <summary>Whether the adjustment has been fully executed.</summary>
    public bool IsCompleted { get; private set; }

    /// <summary>Completion timestamp.</summary>
    public DateTime? CompletionTime { get; private set; }

    /// <summary>Remark — optional note.</summary>
    public string? Remark { get; private set; }

    /// <summary>Adjustment line items.</summary>
    public List<InventoryAdjustmentLine> Lines { get; private set; } = new();

    private InventoryAdjustment() { }

    public InventoryAdjustment(
        Guid id,
        string adjustmentNo,
        AdjustmentType adjustmentType,
        string adjustmentReason,
        Guid warehouseId,
        string warehouseCode,
        string? remark = null)
        : base(id)
    {
        AdjustmentNo = adjustmentNo;
        AdjustmentType = adjustmentType;
        AdjustmentReason = adjustmentReason;
        ApprovalStatus = AdjustmentApprovalStatus.Draft;
        WarehouseId = warehouseId;
        WarehouseCode = warehouseCode;
        IsCompleted = false;
        CompletionTime = null;
        Remark = remark;
    }

    /// <summary>Add a line item to the adjustment.</summary>
    public void AddLine(InventoryAdjustmentLine line)
    {
        if (ApprovalStatus != AdjustmentApprovalStatus.Draft)
        {
            throw new BusinessException("WMS:Inventory:AdjustmentNotDraft",
                "Cannot add lines to a non-draft adjustment.");
        }
        Lines.Add(line);
    }

    /// <summary>Remove a line item from the adjustment.</summary>
    public void RemoveLine(Guid lineId)
    {
        if (ApprovalStatus != AdjustmentApprovalStatus.Draft)
        {
            throw new BusinessException("WMS:Inventory:AdjustmentNotDraft",
                "Cannot remove lines from a non-draft adjustment.");
        }
        var line = Lines.FirstOrDefault(l => l.Id == lineId);
        if (line != null)
        {
            Lines.Remove(line);
        }
    }

    /// <summary>Submit the adjustment for approval.</summary>
    public void Submit()
    {
        if (ApprovalStatus != AdjustmentApprovalStatus.Draft)
        {
            throw new BusinessException("WMS:Inventory:AdjustmentNotDraft",
                "Only draft adjustments can be submitted.");
        }
        if (Lines.Count == 0)
        {
            throw new BusinessException("WMS:Inventory:AdjustmentNoLines",
                "Cannot submit an adjustment with no lines.");
        }
        ApprovalStatus = AdjustmentApprovalStatus.Submitted;
    }

    /// <summary>Approve the adjustment.</summary>
    public void Approve()
    {
        if (ApprovalStatus != AdjustmentApprovalStatus.Submitted)
        {
            throw new BusinessException("WMS:Inventory:AdjustmentNotSubmitted",
                "Only submitted adjustments can be approved.");
        }
        ApprovalStatus = AdjustmentApprovalStatus.Approved;
    }

    /// <summary>Reject the adjustment.</summary>
    public void Reject()
    {
        if (ApprovalStatus != AdjustmentApprovalStatus.Submitted)
        {
            throw new BusinessException("WMS:Inventory:AdjustmentNotSubmitted",
                "Only submitted adjustments can be rejected.");
        }
        ApprovalStatus = AdjustmentApprovalStatus.Rejected;
    }

    /// <summary>Execute the adjustment — applies changes to inventory.</summary>
    public void Execute()
    {
        if (ApprovalStatus != AdjustmentApprovalStatus.Approved)
        {
            throw new BusinessException("WMS:Inventory:AdjustmentNotApproved",
                "Only approved adjustments can be executed.");
        }
        IsCompleted = true;
        CompletionTime = DateTime.UtcNow;
        ApprovalStatus = AdjustmentApprovalStatus.Executed;
    }

    /// <summary>Cancel the adjustment.</summary>
    public void Cancel()
    {
        if (ApprovalStatus == AdjustmentApprovalStatus.Executed ||
            ApprovalStatus == AdjustmentApprovalStatus.Cancelled)
        {
            throw new BusinessException("WMS:Inventory:AdjustmentCannotCancel",
                "Cannot cancel an executed or already cancelled adjustment.");
        }
        ApprovalStatus = AdjustmentApprovalStatus.Cancelled;
    }
}
