namespace Wms.Workflow.Domain.Enums;

/// <summary>
/// ApprovalFlowType Smart Enum — defines the types of approval flows.
/// </summary>
public sealed class ApprovalFlowType : Wms.Shared.Domain.Enums.SmartEnum<ApprovalFlowType, int>
{
    public static readonly ApprovalFlowType Inbound = new ApprovalFlowType("Inbound", 0, "入库审批");
    public static readonly ApprovalFlowType Return = new ApprovalFlowType("Return", 1, "退料审批");
    public static readonly ApprovalFlowType DifferenceAdjustment = new ApprovalFlowType("DifferenceAdjustment", 2, "差异调整审批");
    public static readonly ApprovalFlowType Transfer = new ApprovalFlowType("Transfer", 3, "调拨审批");
    public static readonly ApprovalFlowType MaterialRequisition = new ApprovalFlowType("MaterialRequisition", 4, "领料审批");

    public string Description { get; }

    private ApprovalFlowType(string name, int value, string description) : base(name, value)
    {
        Description = description;
    }
}
