using Wms.Shared.Domain.Events;

namespace Wms.Notification.Domain.Events;

/// <summary>
/// Approval Completed Event stub — placeholder for Workflow module events
/// </summary>
public class ApprovalCompletedEvent : EventDataBase
{
    public Guid ApprovalId { get; set; }
    public string ApprovalNo { get; set; } = string.Empty;
    public bool IsApproved { get; set; }
    public string? Comment { get; set; }
}
