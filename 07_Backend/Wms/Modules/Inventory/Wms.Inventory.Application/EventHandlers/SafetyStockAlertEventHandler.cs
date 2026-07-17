using Wms.Inventory.Domain.Aggregates;
using Wms.Inventory.Domain.Enums;
using Wms.Inventory.Domain.Events;
using Wms.Inventory.Domain.Repositories;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus;

namespace Wms.Inventory.Application.EventHandlers;

/// <summary>
/// Safety Stock Alert Event Handler — creates InventoryAlert when safety stock is breached.
/// (DE-002, Phase 3 DDD Design)
/// </summary>
public class SafetyStockAlertEventHandler : ILocalEventHandler<SafetyStockAlertEvent>, ITransientDependency
{
    private readonly IInventoryAlertRepository _alertRepository;

    public SafetyStockAlertEventHandler(IInventoryAlertRepository alertRepository)
    {
        _alertRepository = alertRepository;
    }

    public async Task HandleEventAsync(SafetyStockAlertEvent eventData)
    {
        var alert = new InventoryAlert(
            GuidGenerator.Create(),
            AlertType.SafetyStock,
            eventData.MaterialId,
            eventData.MaterialCode,
            eventData.WarehouseId,
            eventData.WarehouseCode,
            eventData.CurrentAvailable,
            eventData.SafetyStockQuantity);

        await _alertRepository.InsertAsync(alert);
    }

    private IGuidGenerator GuidGenerator => new SimpleGuidGenerator();
}
