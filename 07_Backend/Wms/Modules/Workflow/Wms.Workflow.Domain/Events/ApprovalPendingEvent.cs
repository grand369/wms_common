using System;

namespace Wms.Workflow.Domain.Events;

/// <summary>
/// DE-035: ApprovalPendingEvent — raised when an approval instance enters a pending/approval state.
/// Notifies the assigned approver.
/// </summary>
public class ApprovalPendingEvent : EventDataBase
{
    public Guid InstanceId { get; }
    public Guid FlowId { get; }
    public Guid OrderId { get; }
    public string OrderType { get; }
    public Guid ApproverId { get; }
    public string ApproverName { get; }

    public ApprovalPendingEvent(
        Guid instanceId,
        Guid flowId,
        Guid orderId,
        string orderType,
        Guid approverId,
        string approverName)
    {
        InstanceId = instanceId;
        FlowId = flowId;
        OrderId = orderId;
        OrderType = orderType;
        ApproverId = approverId;
        ApproverName = approverName;
    }
}
