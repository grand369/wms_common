using Volo.Abp.EventBus;
using Wms.Material.Domain.Events;

namespace Wms.Material.Application.EventHandlers;

/// <summary>
/// Material Created Event Handler — handles the MaterialCreatedEvent.
/// Currently a placeholder for future notification triggers.
/// (Phase 3 DDD Design)
/// </summary>
public class MaterialCreatedEventHandler : ILocalEventHandler<MaterialCreatedEvent>
{
    public MaterialCreatedEventHandler()
    {
    }

    public async Task HandleEventAsync(MaterialCreatedEvent eventData)
    {
        // Placeholder: future implementation could trigger notification to relevant users
        // e.g., send notification to inventory manager that a new material was created
        await Task.CompletedTask;
    }
}
