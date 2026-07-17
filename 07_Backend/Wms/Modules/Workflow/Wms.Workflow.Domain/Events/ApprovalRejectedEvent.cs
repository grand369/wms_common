using System;

namespace Wms.Workflow.Domain.Events;

/// <summary>
/// ApprovalRejectedEvent — raised when an approval instance is rejected.
/// </summary>
public class ApprovalRejectedEvent : EventDataBase
{
    public Guid InstanceId { get; }
    public Guid FlowId { get; }
    public Guid OrderId { get; }
    public string OrderType { get; }
    public string RejectReason { get; }

    public ApprovalRejectedEvent(
        Guid instanceId,
        Guid flowId,
        Guid orderId,
        string orderType,
        string rejectReason)
    {
        InstanceId = instanceId;
        FlowId = flowId;
        OrderId = orderId;
        OrderType = orderType;
        RejectReason = rejectReason;
    }
}
