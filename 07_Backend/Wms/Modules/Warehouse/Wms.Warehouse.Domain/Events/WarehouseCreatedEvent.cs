using Wms.Shared.Domain.Events;

namespace Wms.Warehouse.Domain.Events;

/// <summary>
/// Warehouse Created Event — raised when a new warehouse is created.
/// (Phase 3 DDD Design)
/// </summary>
public class WarehouseCreatedEvent : EventDataBase
{
    public Guid WarehouseId { get; set; }
    public string WarehouseCode { get; set; } = string.Empty;
    public string WarehouseName { get; set; } = string.Empty;
    public int WarehouseType { get; set; }
    public Guid OrganizationUnitId { get; set; }
}
