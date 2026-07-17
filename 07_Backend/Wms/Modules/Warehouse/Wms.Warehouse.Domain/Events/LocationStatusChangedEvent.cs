using Wms.Shared.Domain.Events;

namespace Wms.Warehouse.Domain.Events;

/// <summary>
/// Location Status Changed Event — raised when a location is activated or deactivated.
/// (Phase 3 DDD Design)
/// </summary>
public class LocationStatusChangedEvent : EventDataBase
{
    public Guid LocationId { get; set; }
    public string LocationCode { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}
