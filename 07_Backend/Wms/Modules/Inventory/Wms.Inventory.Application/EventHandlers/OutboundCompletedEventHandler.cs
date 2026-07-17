using Wms.Inventory.Domain.Services;
using Wms.Inventory.Domain.Enums;
using Wms.Shared.Domain.Enums;
using Wms.Shared.Domain.Events;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus;

namespace Wms.Inventory.Application.EventHandlers;

/// <summary>
/// Outbound Completed Event Handler — when outbound order is completed,
/// calls InventoryDomainService.DecreaseInventoryAsync to deduct stock.
/// (DE-018, Phase 3 DDD Design)
/// </summary>
public class OutboundCompletedEventHandler : ILocalEventHandler<OutboundCompletedEvent>, ITransientDependency
{
    private readonly InventoryDomainService _domainService;

    public OutboundCompletedEventHandler(InventoryDomainService domainService)
    {
        _domainService = domainService;
    }

    public async Task HandleEventAsync(OutboundCompletedEvent eventData)
    {
        foreach (var line in eventData.Lines)
        {
            await _domainService.DecreaseInventoryAsync(
                line.MaterialId,
                eventData.WarehouseId,
                line.LocationId,
                line.BatchNumber,
                InventoryStatus.Available,
                line.Quantity,
                "OutboundOrder",
                eventData.OrderId,
                eventData.AllowNegativeInventory);
        }
    }
}

/// <summary>
/// Outbound Completed Event Data — placeholder for the event from Outbound module.
/// </summary>
public class OutboundCompletedEvent : EventDataBase
{
    public Guid OrderId { get; set; }
    public Guid WarehouseId { get; set; }
    public bool AllowNegativeInventory { get; set; } = false;
    public List<OutboundCompletedLine> Lines { get; set; } = new();
}

public class OutboundCompletedLine
{
    public Guid MaterialId { get; set; }
    public Guid LocationId { get; set; }
    public decimal Quantity { get; set; }
    public string? BatchNumber { get; set; }
}
