using Wms.Shared.Domain.Events;

namespace Wms.Notification.Domain.Events;

/// <summary>
/// Approval Pending Event stub — placeholder for Workflow module events
/// </summary>
public class ApprovalPendingEvent : EventDataBase
{
    public Guid ApprovalId { get; set; }
    public string ApprovalNo { get; set; } = string.Empty;
    public string ApprovalType { get; set; } = string.Empty;
    public Guid CurrentApproverId { get; set; }
    public string CurrentApproverName { get; set; } = string.Empty;
}
