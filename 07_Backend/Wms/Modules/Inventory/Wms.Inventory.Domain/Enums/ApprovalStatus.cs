using Wms.Shared.Domain.Enums;

namespace Wms.Inventory.Domain.Enums;

/// <summary>
/// Approval Status SmartEnum — defines the approval workflow states.
/// Used by InventoryAdjustment and InventoryFreezeOrder.
/// </summary>
public sealed class ApprovalStatus : SmartEnum<ApprovalStatus, int>
{
    public static readonly ApprovalStatus Draft =
        new ApprovalStatus("Draft", 0, "草稿");
    public static readonly ApprovalStatus Submitted =
        new ApprovalStatus("Submitted", 1, "已提交");
    public static readonly ApprovalStatus Approved =
        new ApprovalStatus("Approved", 2, "已审批");
    public static readonly ApprovalStatus Rejected =
        new ApprovalStatus("Rejected", 3, "已驳回");
    public static readonly ApprovalStatus Cancelled =
        new ApprovalStatus("Cancelled", 4, "已取消");

    public string Description { get; }

    private ApprovalStatus(string name, int value, string description) : base(name, value)
    {
        Description = description;
    }
}
