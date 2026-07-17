using Volo.Abp.EventBus;

namespace Wms.Shared.Domain.Events;

/// <summary>
/// Event Data Base Class — provides common fields for all domain events.
/// All module-specific event data classes should inherit from this base.
/// </summary>
public abstract class EventDataBase
{
    /// <summary>
    /// The unique identifier for the event instance (for idempotency tracking).
    /// </summary>
    public Guid EventId { get; set; } = Guid.NewGuid();

    /// <summary>
    /// The timestamp when the event was raised.
    /// </summary>
    public DateTime EventTime { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// The source module that raised the event (e.g., "Inventory", "Inbound").
    /// </summary>
    public string SourceModule { get; set; } = string.Empty;

    /// <summary>
    /// The aggregate root ID that triggered the event.
    /// </summary>
    public Guid AggregateRootId { get; set; }
}
