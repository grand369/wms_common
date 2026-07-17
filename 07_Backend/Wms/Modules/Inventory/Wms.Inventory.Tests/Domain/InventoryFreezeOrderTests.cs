using Shouldly;
using Wms.Inventory.Domain.Aggregates;
using Wms.Inventory.Domain.Enums;
using Xunit;

namespace Wms.Inventory.Tests.Domain;

/// <summary>
/// Inventory Freeze Order Tests — verifies state transitions (Active → Released/Cancelled).
/// </summary>
public class InventoryFreezeOrderTests
{
    [Fact]
    public void Create_FreezeOrder_ShouldBeActive()
    {
        var freeze = new InventoryFreezeOrder(
            Guid.NewGuid(),
            "FRZ-001",
            FreezeScope.ByBatch,
            "质量疑问冻结",
            Guid.NewGuid(),
            "WH-001",
            DateTime.UtcNow);

        freeze.FreezeStatus.ShouldBe(FreezeStatus.Active);
        freeze.IsApproved.ShouldBeFalse();
    }

    [Fact]
    public void Approve_FreezeOrder_ShouldSetApproved()
    {
        var freeze = new InventoryFreezeOrder(
            Guid.NewGuid(), "FRZ-002", FreezeScope.ByMaterial, "原因",
            Guid.NewGuid(), "WH-001", DateTime.UtcNow);

        freeze.Approve();

        freeze.IsApproved.ShouldBeTrue();
    }

    [Fact]
    public void Release_FreezeOrder_ShouldChangeToReleased()
    {
        var freeze = new InventoryFreezeOrder(
            Guid.NewGuid(), "FRZ-003", FreezeScope.ByLocation, "原因",
            Guid.NewGuid(), "WH-001", DateTime.UtcNow);

        freeze.Release();

        freeze.FreezeStatus.ShouldBe(FreezeStatus.Released);
    }

    [Fact]
    public void Cancel_FreezeOrder_ShouldChangeToCancelled()
    {
        var freeze = new InventoryFreezeOrder(
            Guid.NewGuid(), "FRZ-004", FreezeScope.ByWarehouse, "原因",
            Guid.NewGuid(), "WH-001", DateTime.UtcNow);

        freeze.Cancel();

        freeze.FreezeStatus.ShouldBe(FreezeStatus.Cancelled);
    }

    [Fact]
    public void Cancel_ReleasedFreezeOrder_ShouldThrowException()
    {
        var freeze = new InventoryFreezeOrder(
            Guid.NewGuid(), "FRZ-005", FreezeScope.ByBatch, "原因",
            Guid.NewGuid(), "WH-001", DateTime.UtcNow);

        freeze.Release();

        Should.Throw<BusinessException>(() => freeze.Cancel());
    }
}
