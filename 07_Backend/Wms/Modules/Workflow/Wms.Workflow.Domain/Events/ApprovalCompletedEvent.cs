using System;

namespace Wms.Workflow.Domain.Events;

/// <summary>
/// DE-036: ApprovalCompletedEvent — raised when an approval instance is completed (Approved or Rejected).
/// </summary>
public class ApprovalCompletedEvent : EventDataBase
{
    public Guid InstanceId { get; }
    public Guid FlowId { get; }
    public Guid OrderId { get; }
    public string OrderType { get; }
    /// <summary>"Approved" or "Rejected"</summary>
    public string Result { get; }

    public ApprovalCompletedEvent(
        Guid instanceId,
        Guid flowId,
        Guid orderId,
        string orderType,
        string result)
    {
        InstanceId = instanceId;
        FlowId = flowId;
        OrderId = orderId;
        OrderType = orderType;
        Result = result;
    }
}
