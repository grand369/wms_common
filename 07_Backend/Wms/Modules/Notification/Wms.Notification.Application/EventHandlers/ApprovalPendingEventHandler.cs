using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus;
using Wms.Notification.Domain.Enums;
using Wms.Notification.Domain.Services;
using ApprovalPendingEvent = Wms.Notification.Domain.Events.ApprovalPendingEvent;

namespace Wms.Notification.Application.EventHandlers;

/// <summary>
/// Approval Pending Event Handler — sends approval pending notification.
/// </summary>
public class ApprovalPendingEventHandler : ILocalEventHandler<ApprovalPendingEvent>, ITransientDependency
{
    private readonly NotificationDomainService _notificationDomainService;

    public ApprovalPendingEventHandler(NotificationDomainService notificationDomainService)
    {
        _notificationDomainService = notificationDomainService;
    }

    public async Task HandleEventAsync(ApprovalPendingEvent eventData)
    {
        await _notificationDomainService.CreateAndSendAsync(
            NotificationType.Approval,
            NotificationChannel.Internal,
            "待审批通知",
            $"审批单 {eventData.ApprovalNo} 等待您审批，类型：{eventData.ApprovalType}",
            eventData.CurrentApproverId,
            eventData.CurrentApproverName,
            NotificationPriority.High,
            "ApprovalPending",
            "Workflow");
    }
}
