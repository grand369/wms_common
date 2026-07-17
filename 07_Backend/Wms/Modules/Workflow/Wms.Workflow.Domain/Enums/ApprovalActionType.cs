namespace Wms.Workflow.Domain.Enums;

/// <summary>
/// ApprovalActionType Smart Enum — defines the types of actions in an approval process.
/// </summary>
public sealed class ApprovalActionType : Wms.Shared.Domain.Enums.SmartEnum<ApprovalActionType, int>
{
    public static readonly ApprovalActionType Approve = new ApprovalActionType("Approve", 0, "通过");
    public static readonly ApprovalActionType Reject = new ApprovalActionType("Reject", 1, "驳回");
    public static readonly ApprovalActionType Resubmit = new ApprovalActionType("Resubmit", 2, "重新提交");
    public static readonly ApprovalActionType Cancel = new ApprovalActionType("Cancel", 3, "取消");
    public static readonly ApprovalActionType Delegate = new ApprovalActionType("Delegate", 4, "委托");

    public string Description { get; }

    private ApprovalActionType(string name, int value, string description) : base(name, value)
    {
        Description = description;
    }
}
