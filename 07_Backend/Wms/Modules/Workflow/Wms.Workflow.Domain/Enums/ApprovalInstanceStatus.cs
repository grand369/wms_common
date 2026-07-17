namespace Wms.Workflow.Domain.Enums;

/// <summary>
/// ApprovalInstanceStatus Smart Enum — defines the statuses of an approval instance.
/// </summary>
public sealed class ApprovalInstanceStatus : Wms.Shared.Domain.Enums.SmartEnum<ApprovalInstanceStatus, int>
{
    public static readonly ApprovalInstanceStatus Pending = new ApprovalInstanceStatus("Pending", 0, "待审批");
    public static readonly ApprovalInstanceStatus InProgress = new ApprovalInstanceStatus("InProgress", 1, "审批中");
    public static readonly ApprovalInstanceStatus Approved = new ApprovalInstanceStatus("Approved", 2, "已通过");
    public static readonly ApprovalInstanceStatus Rejected = new ApprovalInstanceStatus("Rejected", 3, "已驳回");
    public static readonly ApprovalInstanceStatus Resubmitted = new ApprovalInstanceStatus("Resubmitted", 4, "已重新提交");
    public static readonly ApprovalInstanceStatus Cancelled = new ApprovalInstanceStatus("Cancelled", 5, "已取消");

    public string Description { get; }

    private ApprovalInstanceStatus(string name, int value, string description) : base(name, value)
    {
        Description = description;
    }
}
