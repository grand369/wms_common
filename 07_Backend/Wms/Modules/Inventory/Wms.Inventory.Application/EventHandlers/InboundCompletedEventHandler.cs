using Wms.Inventory.Domain.Services;
using Wms.Inventory.Domain.Enums;
using Wms.Shared.Domain.Enums;
using Wms.Shared.Domain.Events;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus;

namespace Wms.Inventory.Application.EventHandlers;

/// <summary>
/// Inbound Completed Event Handler — when inbound order is completed,
/// calls InventoryDomainService.IncreaseInventoryAsync to add stock.
/// (DE-012, Phase 3 DDD Design)
/// </summary>
public class InboundCompletedEventHandler : ILocalEventHandler<InboundCompletedEvent>, ITransientDependency
{
    private readonly InventoryDomainService _domainService;

    public InboundCompletedEventHandler(InventoryDomainService domainService)
    {
        _domainService = domainService;
    }

    public async Task HandleEventAsync(InboundCompletedEvent eventData)
    {
        // Process each inbound line — increase inventory for each material
        foreach (var line in eventData.Lines)
        {
            await _domainService.IncreaseInventoryAsync(
                line.MaterialId,
                eventData.WarehouseId,
                line.LocationId,
                line.BatchNumber,
                line.Quantity,
                line.MaterialCode,
                eventData.WarehouseCode,
                line.LocationCode,
                "InboundOrder",
                eventData.OrderId,
                eventData.AllowNegativeInventory);
        }
    }
}

/// <summary>
/// Inbound Completed Event Data — placeholder for the event from Inbound module.
/// In production, this would be defined in Wms.Inbound.Application.Contracts.
/// </summary>
public class InboundCompletedEvent : EventDataBase
{
    public Guid OrderId { get; set; }
    public Guid WarehouseId { get; set; }
    public string WarehouseCode { get; set; } = string.Empty;
    public bool AllowNegativeInventory { get; set; } = false;
    public List<InboundCompletedLine> Lines { get; set; } = new();
}

public class InboundCompletedLine
{
    public Guid MaterialId { get; set; }
    public string MaterialCode { get; set; } = string.Empty;
    public Guid LocationId { get; set; }
    public string LocationCode { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public string? BatchNumber { get; set; }
}
