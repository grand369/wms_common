using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus;
using Wms.Workflow.Domain.Events;

namespace Wms.Workflow.Application.EventHandlers;

/// <summary>
/// DE-036: ApprovalCompletedEventHandler —
/// Handles approval completion events, notifies business modules of the result.
/// </summary>
public class ApprovalCompletedEventHandler : ILocalEventHandler<ApprovalCompletedEvent>, ITransientDependency
{
    public async Task HandleEventAsync(ApprovalCompletedEvent eventData)
    {
        // TODO: Notify business module of approval result via domain events
        // The business module (e.g., Transfer, Inbound) should subscribe to this event
        // and proceed with the next step (e.g., confirm outbound, complete order, etc.)

        await Task.CompletedTask;
    }
}
