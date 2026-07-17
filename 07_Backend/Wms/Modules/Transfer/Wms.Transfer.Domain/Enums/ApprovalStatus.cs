namespace Wms.Transfer.Domain.Enums;

/// <summary>
/// Approval Status Smart Enum — tracks the approval workflow state for transfer orders.
/// </summary>
public sealed class ApprovalStatus : Wms.Shared.Domain.Enums.SmartEnum<ApprovalStatus, int>
{
    public static readonly ApprovalStatus None = new ApprovalStatus("None", 0, "无需审批");
    public static readonly ApprovalStatus Pending = new ApprovalStatus("Pending", 1, "待审批");
    public static readonly ApprovalStatus Approved = new ApprovalStatus("Approved", 2, "审批通过");
    public static readonly ApprovalStatus Rejected = new ApprovalStatus("Rejected", 3, "审批驳回");

    public string Description { get; }

    private ApprovalStatus(string name, int value, string description) : base(name, value)
    {
        Description = description;
    }
}
