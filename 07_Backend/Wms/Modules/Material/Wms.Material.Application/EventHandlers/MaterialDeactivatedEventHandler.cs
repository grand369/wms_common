using Volo.Abp.EventBus;
using Wms.Material.Domain.Events;

namespace Wms.Material.Application.EventHandlers;

/// <summary>
/// Material Deactivated Event Handler — handles the MaterialDeactivatedEvent.
/// Currently a placeholder for future notification triggers.
/// (Phase 3 DDD Design)
/// </summary>
public class MaterialDeactivatedEventHandler : ILocalEventHandler<MaterialDeactivatedEvent>
{
    public MaterialDeactivatedEventHandler()
    {
    }

    public async Task HandleEventAsync(MaterialDeactivatedEvent eventData)
    {
        // Placeholder: future implementation could trigger inventory check or notification
        await Task.CompletedTask;
    }
}
