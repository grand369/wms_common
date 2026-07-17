using Wms.Shared.Domain.Events;

namespace Wms.Warehouse.Domain.Events;

/// <summary>
/// Warehouse Deactivated Event — raised when a warehouse is deactivated.
/// (Phase 3 DDD Design)
/// </summary>
public class WarehouseDeactivatedEvent : EventDataBase
{
    public Guid WarehouseId { get; set; }
    public string WarehouseCode { get; set; } = string.Empty;
}
