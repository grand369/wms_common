using Wms.Outbound.Domain.Events;
using Volo.Abp.EventBus;
using Microsoft.Extensions.Logging;

namespace Wms.Outbound.Application.EventHandlers;

/// <summary>
/// OutboundCompletedEventHandler — handles OutboundCompletedEvent (DE-018).
/// v1.0 placeholder for ERP callback. In v1.1, will trigger ERP synchronization.
/// </summary>
public class OutboundCompletedEventHandler : ILocalEventHandler<OutboundCompletedEvent>
{
    private readonly ILogger<OutboundCompletedEventHandler> _logger;

    public OutboundCompletedEventHandler(ILogger<OutboundCompletedEventHandler> logger)
    {
        _logger = logger;
    }

    public async Task HandleEventAsync(OutboundCompletedEvent eventData)
    {
        _logger.LogInformation(
            "OutboundCompletedEvent received: OrderId={OrderId}, Type={TypeValue}, TotalQty={TotalQty}. " +
            "ERP callback placeholder — will be implemented in v1.1.",
            eventData.OrderId, eventData.OutboundTypeValue, eventData.TotalQuantity);

        await Task.CompletedTask;
    }
}
