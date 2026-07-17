using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus;
using Wms.Notification.Domain.Enums;
using Wms.Notification.Domain.Services;
using ApprovalCompletedEvent = Wms.Notification.Domain.Events.ApprovalCompletedEvent;

namespace Wms.Notification.Application.EventHandlers;

/// <summary>
/// Approval Completed Event Handler — sends approval result notification.
/// </summary>
public class ApprovalCompletedEventHandler : ILocalEventHandler<ApprovalCompletedEvent>, ITransientDependency
{
    private readonly NotificationDomainService _notificationDomainService;

    public ApprovalCompletedEventHandler(NotificationDomainService notificationDomainService)
    {
        _notificationDomainService = notificationDomainService;
    }

    public async Task HandleEventAsync(ApprovalCompletedEvent eventData)
    {
        var result = eventData.IsApproved ? "已通过" : "已驳回";
        await _notificationDomainService.CreateAndSendAsync(
            NotificationType.Approval,
            NotificationChannel.Internal,
            "审批结果通知",
            $"审批单 {eventData.ApprovalNo} {result}" + (eventData.Comment != null ? $"，审批意见：{eventData.Comment}" : ""),
            Guid.Empty,
            "申请人",
            NotificationPriority.Normal,
            "ApprovalCompleted",
            "Workflow");
    }
}
