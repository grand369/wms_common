using Shouldly;
using Wms.Warehouse.Domain.Aggregates;
using Wms.Warehouse.Domain.Enums;

namespace Wms.Warehouse.Tests.Domain;

/// <summary>
/// Warehouse Area Domain Tests — tests aggregate root creation, function setting, capacity update, activation/deactivation.
/// (Phase 8 Coding Conventions, Section 6)
/// </summary>
public class WarehouseAreaTests
{
    [Fact]
    public void Create_WarehouseArea_Should_Set_All_Properties()
    {
        var warehouseId = "WH-ID-001";
        var area = new WarehouseArea(
            Guid.NewGuid(),
            "A-01",
            "收货区",
            warehouseId,
            "WH-001",
            AreaFunction.Receiving.Value,
            StorageEnvironment.Normal.Value,
            1000,
            0,
            true);

        area.AreaCode.ShouldBe("A-01");
        area.AreaName.ShouldBe("收货区");
        area.WarehouseId.ShouldBe(warehouseId);
        area.WarehouseCode.ShouldBe("WH-001");
        area.AreaFunction.ShouldBe(AreaFunction.Receiving.Value);
        area.StorageEnvironment.ShouldBe(StorageEnvironment.Normal.Value);
        area.MaxCapacity.ShouldBe(1000);
        area.CurrentCapacity.ShouldBe(0);
        area.IsActive.ShouldBeTrue();
    }

    [Fact]
    public void SetAreaCode_Should_Update_Code()
    {
        var area = CreateSampleArea();
        area.SetAreaCode("A-02");
        area.AreaCode.ShouldBe("A-02");
    }

    [Fact]
    public void SetAreaCode_Empty_Should_Throw()
    {
        var area = CreateSampleArea();
        Should.Throw<ArgumentException>(() => area.SetAreaCode(""));
    }

    [Fact]
    public void SetAreaFunction_Should_Update_Function()
    {
        var area = CreateSampleArea();
        area.SetAreaFunction(AreaFunction.Storage.Value);
        area.AreaFunction.ShouldBe(AreaFunction.Storage.Value);
    }

    [Fact]
    public void SetAreaFunction_Invalid_Should_Throw()
    {
        var area = CreateSampleArea();
        Should.Throw<ArgumentException>(() => area.SetAreaFunction(999));
    }

    [Fact]
    public void UpdateCapacity_Should_Update_Both_Capacities()
    {
        var area = CreateSampleArea();
        area.UpdateCapacity(2000, 500);
        area.MaxCapacity.ShouldBe(2000);
        area.CurrentCapacity.ShouldBe(500);
    }

    [Fact]
    public void UpdateCapacity_CurrentExceedsMax_Should_Throw()
    {
        var area = CreateSampleArea();
        Should.Throw<ArgumentException>(() => area.UpdateCapacity(1000, 2000));
    }

    [Fact]
    public void SetActive_Should_Set_IsActive_True()
    {
        var area = CreateSampleArea();
        area.Deactivate();
        area.IsActive.ShouldBeFalse();
        area.SetActive();
        area.IsActive.ShouldBeTrue();
    }

    [Fact]
    public void Deactivate_Should_Set_IsActive_False()
    {
        var area = CreateSampleArea();
        area.Deactivate();
        area.IsActive.ShouldBeFalse();
    }

    private static WarehouseArea CreateSampleArea()
    {
        return new WarehouseArea(
            Guid.NewGuid(),
            "A-01",
            "收货区",
            "WH-ID-001",
            "WH-001",
            AreaFunction.Receiving.Value);
    }
}
