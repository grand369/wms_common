using Wms.Outbound.Domain.Events;
using Volo.Abp.EventBus;
using Microsoft.Extensions.Logging;

namespace Wms.Outbound.Application.EventHandlers;

/// <summary>
/// OverIssueDetectedEventHandler — handles OverIssueDetectedEvent (DE-020).
/// v1.0 placeholder for alert notification. In v1.1, will trigger alert to warehouse manager.
/// </summary>
public class OverIssueDetectedEventHandler : ILocalEventHandler<OverIssueDetectedEvent>
{
    private readonly ILogger<OverIssueDetectedEventHandler> _logger;

    public OverIssueDetectedEventHandler(ILogger<OverIssueDetectedEventHandler> logger)
    {
        _logger = logger;
    }

    public async Task HandleEventAsync(OverIssueDetectedEvent eventData)
    {
        _logger.LogWarning(
            "OverIssueDetectedEvent: OrderId={OrderId}, MaterialId={MaterialId}, " +
            "RequiredQty={RequiredQty}, ActualQty={ActualQty}. " +
            "Alert placeholder — will notify warehouse manager in v1.1.",
            eventData.OrderId, eventData.MaterialId,
            eventData.RequiredQuantity, eventData.ActualQuantity);

        await Task.CompletedTask;
    }
}
