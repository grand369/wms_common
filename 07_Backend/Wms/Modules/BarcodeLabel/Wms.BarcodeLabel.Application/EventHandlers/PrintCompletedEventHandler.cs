using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Volo.Abp.EventBus;
using Wms.BarcodeLabel.Domain.Events;

namespace Wms.BarcodeLabel.Application.EventHandlers;

/// <summary>
/// PrintCompletedEventHandler — handles PrintCompletedEvent (DE-034).
/// Can update source order status or trigger downstream notifications.
/// v1.0 placeholder for order status sync. In v1.1, will update source order status.
/// </summary>
public class PrintCompletedEventHandler : ILocalEventHandler<PrintCompletedEvent>
{
    private readonly ILogger<PrintCompletedEventHandler> _logger;

    public PrintCompletedEventHandler(ILogger<PrintCompletedEventHandler> logger)
    {
        _logger = logger;
    }

    public async Task HandleEventAsync(PrintCompletedEvent eventData)
    {
        _logger.LogInformation(
            "PrintCompletedEvent received: PrintTaskId={PrintTaskId}, TaskNo={TaskNo}, PrinterId={PrinterId}. " +
            "Source order status sync placeholder — will be implemented in v1.1.",
            eventData.PrintTaskId, eventData.TaskNo, eventData.PrinterId);

        // v1.0 placeholder — no actual source order status update
        await Task.CompletedTask;
    }
}
