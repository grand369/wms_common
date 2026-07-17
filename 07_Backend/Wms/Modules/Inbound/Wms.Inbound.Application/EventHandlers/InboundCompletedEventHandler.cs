using Wms.Inbound.Domain.Events;
using Volo.Abp.EventBus;
using Volo.Abp.Logging;
using Microsoft.Extensions.Logging;

namespace Wms.Inbound.Application.EventHandlers;

/// <summary>
/// InboundCompletedEventHandler — handles InboundCompletedEvent (DE-012).
/// v1.0 placeholder for ERP callback. In v1.1, will trigger ERP synchronization.
/// </summary>
public class InboundCompletedEventHandler : ILocalEventHandler<InboundCompletedEvent>
{
    private readonly ILogger<InboundCompletedEventHandler> _logger;

    public InboundCompletedEventHandler(ILogger<InboundCompletedEventHandler> logger)
    {
        _logger = logger;
    }

    public async Task HandleEventAsync(InboundCompletedEvent eventData)
    {
        _logger.LogInformation(
            "InboundCompletedEvent received: OrderId={OrderId}, Type={TypeValue}, TotalQty={TotalQty}. " +
            "ERP callback placeholder — will be implemented in v1.1.",
            eventData.OrderId, eventData.InboundTypeValue, eventData.TotalQuantity);

        // v1.0 placeholder — no actual ERP callback
        await Task.CompletedTask;
    }
}
