using Wms.Shared.Domain.Events;

namespace Wms.Material.Domain.Events;

/// <summary>
/// Material Deactivated Event — raised when a material is deactivated.
/// (Phase 3 DDD Design)
/// </summary>
public class MaterialDeactivatedEvent : EventDataBase
{
    public Guid MaterialId { get; set; }
    public string MaterialCode { get; set; } = string.Empty;
}
