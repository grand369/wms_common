using Shouldly;
using Wms.Shared.Domain.Events;
using Xunit;

namespace Wms.Shared.Tests.Domain;

/// <summary>
/// EventDataBase Tests — verifies the cross-module event data base class.
/// Tests construction, property defaults, and idempotency tracking fields.
/// (Phase 10 — Shared Kernel core tests)
/// </summary>
public class EventDataBaseTests
{
    [Fact]
    public void EventDataBase_ShouldHaveDefaultIdAndTimestamp()
    {
        var eventData = new TestEventData();

        eventData.EventId.ShouldNotBe(Guid.Empty);
        eventData.EventTime.ShouldNotBe(default);
    }

    [Fact]
    public void EventDataBase_Id_ShouldBeUniquePerInstance()
    {
        var event1 = new TestEventData();
        var event2 = new TestEventData();

        event1.EventId.ShouldNotBe(event2.EventId);
    }

    [Fact]
    public void EventDataBase_SourceModule_ShouldDefaultEmpty()
    {
        var eventData = new TestEventData();
        eventData.SourceModule.ShouldBe(string.Empty);
    }

    [Fact]
    public void EventDataBase_AggregateRootId_ShouldDefaultEmptyGuid()
    {
        var eventData = new TestEventData();
        eventData.AggregateRootId.ShouldBe(Guid.Empty);
    }

    [Fact]
    public void EventDataBase_CanSetProperties()
    {
        var aggregateId = Guid.NewGuid();
        var eventData = new TestEventData
        {
            SourceModule = "Inventory",
            AggregateRootId = aggregateId
        };

        eventData.SourceModule.ShouldBe("Inventory");
        eventData.AggregateRootId.ShouldBe(aggregateId);
    }

    private class TestEventData : EventDataBase { }
}
