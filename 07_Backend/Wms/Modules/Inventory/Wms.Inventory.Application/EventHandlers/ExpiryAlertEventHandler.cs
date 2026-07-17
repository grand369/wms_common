using Wms.Inventory.Domain.Aggregates;
using Wms.Inventory.Domain.Enums;
using Wms.Inventory.Domain.Events;
using Wms.Inventory.Domain.Repositories;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus;

namespace Wms.Inventory.Application.EventHandlers;

/// <summary>
/// Expiry Alert Event Handler — creates InventoryAlert when inventory is near expiry.
/// (DE-003, Phase 3 DDD Design)
/// </summary>
public class ExpiryAlertEventHandler : ILocalEventHandler<ExpiryAlertEvent>, ITransientDependency
{
    private readonly IInventoryAlertRepository _alertRepository;

    public ExpiryAlertEventHandler(IInventoryAlertRepository alertRepository)
    {
        _alertRepository = alertRepository;
    }

    public async Task HandleEventAsync(ExpiryAlertEvent eventData)
    {
        var alert = new InventoryAlert(
            GuidGenerator.Create(),
            AlertType.Expiry,
            eventData.MaterialId,
            eventData.MaterialCode,
            eventData.WarehouseId,
            eventData.WarehouseCode,
            0, // Current quantity not available in event
            eventData.DaysLeft);

        await _alertRepository.InsertAsync(alert);
    }

    private IGuidGenerator GuidGenerator => new SimpleGuidGenerator();
}
