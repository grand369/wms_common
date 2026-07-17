using Wms.Shared.Domain.Events;

namespace Wms.Warehouse.Domain.Events;

/// <summary>
/// Location Created Event — raised when a new location is created.
/// (Phase 3 DDD Design)
/// </summary>
public class LocationCreatedEvent : EventDataBase
{
    public Guid LocationId { get; set; }
    public string LocationCode { get; set; } = string.Empty;
    public string WarehouseId { get; set; } = string.Empty;
    public string AreaId { get; set; } = string.Empty;
}
