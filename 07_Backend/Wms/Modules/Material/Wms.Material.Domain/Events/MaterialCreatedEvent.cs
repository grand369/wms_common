using Wms.Shared.Domain.Events;

namespace Wms.Material.Domain.Events;

/// <summary>
/// Material Created Event — raised when a new material is created.
/// (Phase 3 DDD Design)
/// </summary>
public class MaterialCreatedEvent : EventDataBase
{
    public Guid MaterialId { get; set; }
    public string MaterialCode { get; set; } = string.Empty;
    public string MaterialName { get; set; } = string.Empty;
    public int MaterialType { get; set; }
}
