using Wms.Inventory.Domain.Events;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus;

namespace Wms.Inventory.Application.EventHandlers;

/// <summary>
/// Inventory Changed Event Handler — processes InventoryChangedEvent (DE-001).
/// v1.0 placeholder: logs the event. v1.1 will push to SignalR InventoryHub.
/// </summary>
public class InventoryChangedEventHandler : ILocalEventHandler<InventoryChangedEvent>, ITransientDependency
{
    public async Task HandleEventAsync(InventoryChangedEvent eventData)
    {
        // v1.0 placeholder — log and prepare for future SignalR push
        // In v1.1, this will call IInventoryHubService.PushBalanceChangeAsync()
        await Task.CompletedTask;
    }
}
