namespace Wms.Workflow.Domain.Enums;

/// <summary>
/// ApprovalNodeType Smart Enum — defines the types of nodes in an approval flow.
/// </summary>
public sealed class ApprovalNodeType : Wms.Shared.Domain.Enums.SmartEnum<ApprovalNodeType, int>
{
    public static readonly ApprovalNodeType Start = new ApprovalNodeType("Start", 0, "开始");
    public static readonly ApprovalNodeType Approval = new ApprovalNodeType("Approval", 1, "审批");
    public static readonly ApprovalNodeType Condition = new ApprovalNodeType("Condition", 2, "条件");
    public static readonly ApprovalNodeType End = new ApprovalNodeType("End", 3, "结束");

    public string Description { get; }

    private ApprovalNodeType(string name, int value, string description) : base(name, value)
    {
        Description = description;
    }
}
