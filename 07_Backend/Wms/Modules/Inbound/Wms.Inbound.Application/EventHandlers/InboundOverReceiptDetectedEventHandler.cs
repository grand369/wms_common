using Wms.Inbound.Domain.Events;
using Volo.Abp.EventBus;
using Microsoft.Extensions.Logging;

namespace Wms.Inbound.Application.EventHandlers;

/// <summary>
/// InboundOverReceiptDetectedEventHandler — handles InboundOverReceiptDetectedEvent (DE-013).
/// v1.0 placeholder for notification. In v1.1, will trigger notification to warehouse manager.
/// </summary>
public class InboundOverReceiptDetectedEventHandler : ILocalEventHandler<InboundOverReceiptDetectedEvent>
{
    private readonly ILogger<InboundOverReceiptDetectedEventHandler> _logger;

    public InboundOverReceiptDetectedEventHandler(ILogger<InboundOverReceiptDetectedEventHandler> logger)
    {
        _logger = logger;
    }

    public async Task HandleEventAsync(InboundOverReceiptDetectedEvent eventData)
    {
        _logger.LogWarning(
            "OverReceiptDetected: OrderId={OrderId}, MaterialId={MaterialId}, " +
            "PlanQty={PlanQty}, RecvQty={RecvQty}, Ratio={Ratio}. " +
            "Notification placeholder — will be implemented in v1.1.",
            eventData.OrderId, eventData.MaterialId,
            eventData.PlanQuantity, eventData.ReceivedQuantity, eventData.Ratio);

        // v1.0 placeholder — no actual notification
        await Task.CompletedTask;
    }
}
