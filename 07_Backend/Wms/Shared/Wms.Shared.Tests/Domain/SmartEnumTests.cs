using Shouldly;
using Wms.Shared.Domain.Enums;
using Xunit;

namespace Wms.Shared.Tests.Domain;

/// <summary>
/// SmartEnum Tests — verifies the custom SmartEnum base class behavior.
/// Tests value-based equality, lookup by value, and enumeration listing.
/// (Phase 10 — Shared Kernel core tests)
/// </summary>
public class SmartEnumTests
{
    // Use InventoryStatus as a concrete SmartEnum for testing the base class behavior

    [Fact]
    public void SmartEnum_FromValue_ShouldReturnCorrectInstance()
    {
        var status = SmartEnum<InventoryStatus, int>.FromValue(0);
        status.ShouldBe(InventoryStatus.Available);
        status.Value.ShouldBe(0);
    }

    [Fact]
    public void SmartEnum_FromName_ShouldReturnCorrectInstance()
    {
        var status = SmartEnum<InventoryStatus, int>.FromName("Available");
        status.ShouldBe(InventoryStatus.Available);
    }

    [Fact]
    public void SmartEnum_FromInvalidValue_ShouldThrow()
    {
        Should.Throw<InvalidOperationException>(() =>
        {
            SmartEnum<InventoryStatus, int>.FromValue(999);
        });
    }

    [Fact]
    public void SmartEnum_List_ShouldReturnAllValues()
    {
        var list = SmartEnum<InventoryStatus, int>.List;
        list.ShouldNotBeEmpty();
        list.Count.ShouldBeGreaterThanOrEqualTo(5); // Available, QualityHold, Frozen, Scrapped, InTransit
    }

    [Fact]
    public void SmartEnum_Equality_ByValue_ShouldBeTrue()
    {
        var a = InventoryStatus.Available;
        var b = SmartEnum<InventoryStatus, int>.FromValue(0);
        a.ShouldBe(b);
    }

    [Fact]
    public void SmartEnum_Inequality_DifferentValues_ShouldBeTrue()
    {
        var a = InventoryStatus.Available;
        var b = InventoryStatus.Frozen;
        a.ShouldNotBe(b);
    }

    [Fact]
    public void SmartEnum_ToString_ShouldReturnName()
    {
        InventoryStatus.Available.ToString().ShouldBe("Available");
    }
}
