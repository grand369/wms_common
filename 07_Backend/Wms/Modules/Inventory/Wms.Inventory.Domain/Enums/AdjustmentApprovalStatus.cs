using Wms.Shared.Domain.Enums;

namespace Wms.Inventory.Domain.Enums;

/// <summary>
/// Adjustment Approval Status SmartEnum — extended approval status for adjustments.
/// Includes "Executed" state after approval execution.
/// (AGG-08, Phase 3 DDD Design)
/// </summary>
public sealed class AdjustmentApprovalStatus : SmartEnum<AdjustmentApprovalStatus, int>
{
    public static readonly AdjustmentApprovalStatus Draft =
        new AdjustmentApprovalStatus("Draft", 0, "草稿");
    public static readonly AdjustmentApprovalStatus Submitted =
        new AdjustmentApprovalStatus("Submitted", 1, "已提交");
    public static readonly AdjustmentApprovalStatus Approved =
        new AdjustmentApprovalStatus("Approved", 2, "已审批");
    public static readonly AdjustmentApprovalStatus Rejected =
        new AdjustmentApprovalStatus("Rejected", 3, "已驳回");
    public static readonly AdjustmentApprovalStatus Executed =
        new AdjustmentApprovalStatus("Executed", 4, "已执行");
    public static readonly AdjustmentApprovalStatus Cancelled =
        new AdjustmentApprovalStatus("Cancelled", 5, "已取消");

    public string Description { get; }

    private AdjustmentApprovalStatus(string name, int value, string description) : base(name, value)
    {
        Description = description;
    }
}
