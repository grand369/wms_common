using Volo.Abp.EventBus;
using Wms.Material.Domain.Events;

namespace Wms.Material.Application.EventHandlers;

/// <summary>
/// Material Classification Created Event Handler — handles the MaterialClassificationCreatedEvent.
/// Currently a placeholder for future notification triggers.
/// (Phase 3 DDD Design)
/// </summary>
public class MaterialClassificationCreatedEventHandler : ILocalEventHandler<MaterialClassificationCreatedEvent>
{
    public MaterialClassificationCreatedEventHandler()
    {
    }

    public async Task HandleEventAsync(MaterialClassificationCreatedEvent eventData)
    {
        // Placeholder: future implementation could trigger classification tree refresh notification
        await Task.CompletedTask;
    }
}
