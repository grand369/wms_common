using Volo.Abp.EventBus;
using Wms.Warehouse.Domain.Events;

namespace Wms.Warehouse.Application.EventHandlers;

/// <summary>
/// Warehouse Created Event Handler — handles the WarehouseCreatedEvent.
/// Currently a placeholder for future notification triggers.
/// (Phase 3 DDD Design)
/// </summary>
public class WarehouseCreatedEventHandler : ILocalEventHandler<WarehouseCreatedEvent>
{
    public WarehouseCreatedEventHandler()
    {
    }

    public async Task HandleEventAsync(WarehouseCreatedEvent eventData)
    {
        // Placeholder: future implementation could trigger notification to relevant users
        // e.g., send notification to warehouse manager that a new warehouse was created
        await Task.CompletedTask;
    }
}
