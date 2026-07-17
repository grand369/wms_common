using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus;
using Wms.Workflow.Domain.Events;

namespace Wms.Workflow.Application.EventHandlers;

/// <summary>
/// DE-035: ApprovalPendingEventHandler —
/// Handles approval pending events, e.g. push notification via Notification module.
/// </summary>
public class ApprovalPendingEventHandler : ILocalEventHandler<ApprovalPendingEvent>, ITransientDependency
{
    public async Task HandleEventAsync(ApprovalPendingEvent eventData)
    {
        // TODO: Push notification to approver via Notification module (cross-module dependency via domain events)
        // Example: Use INotificationAppService to send notification to eventData.ApproverId

        await Task.CompletedTask;
    }
}
