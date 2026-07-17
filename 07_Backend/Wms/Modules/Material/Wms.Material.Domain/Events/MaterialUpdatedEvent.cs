using Wms.Shared.Domain.Events;

namespace Wms.Material.Domain.Events;

/// <summary>
/// Material Updated Event — raised when a material's attributes are updated.
/// (Phase 3 DDD Design)
/// </summary>
public class MaterialUpdatedEvent : EventDataBase
{
    public Guid MaterialId { get; set; }
    public string MaterialCode { get; set; } = string.Empty;
}
