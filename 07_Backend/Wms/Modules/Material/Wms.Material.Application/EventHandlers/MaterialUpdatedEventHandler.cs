using Volo.Abp.EventBus;
using Wms.Material.Domain.Events;

namespace Wms.Material.Application.EventHandlers;

/// <summary>
/// Material Updated Event Handler — handles the MaterialUpdatedEvent.
/// Currently a placeholder for future notification triggers.
/// (Phase 3 DDD Design)
/// </summary>
public class MaterialUpdatedEventHandler : ILocalEventHandler<MaterialUpdatedEvent>
{
    public MaterialUpdatedEventHandler()
    {
    }

    public async Task HandleEventAsync(MaterialUpdatedEvent eventData)
    {
        // Placeholder: future implementation could trigger ERP sync or notification
        await Task.CompletedTask;
    }
}
