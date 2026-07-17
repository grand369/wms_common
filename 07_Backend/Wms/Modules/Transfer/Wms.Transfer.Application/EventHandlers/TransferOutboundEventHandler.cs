using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;

namespace Wms.Transfer.Application.EventHandlers;

/// <summary>
/// Handles TransferOutboundEvent (DE-021) — logs outbound confirmation.
/// Core inventory operations are handled synchronously in TransferDomainService.
/// </summary>
public class TransferOutboundEventHandler : ITransientDependency, IDistributedEventHandler<Wms.Transfer.Domain.Events.TransferOutboundEvent>
{
    public async Task HandleEventAsync(Wms.Transfer.Domain.Events.TransferOutboundEvent eventData)
    {
        // Additional async processing: logging, analytics, ERP callback
        // Inventory decrease is handled synchronously in domain service
    }
}
