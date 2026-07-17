using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;

namespace Wms.Transfer.Application.EventHandlers;

/// <summary>
/// Handles TransferInTransitTimeoutEvent (DE-023) — ER-011: notifies supervisor.
/// </summary>
public class TransferInTransitTimeoutEventHandler : ITransientDependency, IDistributedEventHandler<Wms.Transfer.Domain.Events.TransferInTransitTimeoutEvent>
{
    public async Task HandleEventAsync(Wms.Transfer.Domain.Events.TransferInTransitTimeoutEvent eventData)
    {
        // TODO: Send notification to warehouse supervisor via Notification module (Phase C)
    }
}
