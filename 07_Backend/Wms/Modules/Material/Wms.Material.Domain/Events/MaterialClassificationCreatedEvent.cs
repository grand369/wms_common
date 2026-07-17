using Wms.Shared.Domain.Events;

namespace Wms.Material.Domain.Events;

/// <summary>
/// Material Classification Created Event — raised when a new classification is created.
/// (Phase 3 DDD Design)
/// </summary>
public class MaterialClassificationCreatedEvent : EventDataBase
{
    public Guid ClassificationId { get; set; }
    public string ClassificationCode { get; set; } = string.Empty;
    public string ClassificationName { get; set; } = string.Empty;
    public int ClassificationLevel { get; set; }
}
