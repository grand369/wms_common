using Wms.Outbound.Domain.Events;
using Volo.Abp.EventBus;
using Microsoft.Extensions.Logging;

namespace Wms.Outbound.Application.EventHandlers;

/// <summary>
/// OutboundShippedEventHandler — handles OutboundShippedEvent (DE-017).
/// v1.0 placeholder for Notification dispatch. In v1.1, will trigger notification to relevant roles.
/// </summary>
public class OutboundShippedEventHandler : ILocalEventHandler<OutboundShippedEvent>
{
    private readonly ILogger<OutboundShippedEventHandler> _logger;

    public OutboundShippedEventHandler(ILogger<OutboundShippedEventHandler> logger)
    {
        _logger = logger;
    }

    public async Task HandleEventAsync(OutboundShippedEvent eventData)
    {
        _logger.LogInformation(
            "OutboundShippedEvent received: OrderId={OrderId}, TotalShippedQty={TotalShippedQty}. " +
            "Notification placeholder — will be implemented in v1.1.",
            eventData.OrderId, eventData.TotalShippedQuantity);

        await Task.CompletedTask;
    }
}
