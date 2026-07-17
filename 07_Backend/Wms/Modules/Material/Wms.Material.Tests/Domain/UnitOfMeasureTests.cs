using Shouldly;
using Wms.Material.Domain.Entities;
using Wms.Material.Domain.Enums;

namespace Wms.Material.Tests.Domain;

/// <summary>
/// Unit of Measure Domain Tests — tests entity creation, code/name setting, activation.
/// (Phase 8 Coding Conventions, Section 6)
/// </summary>
public class UnitOfMeasureTests
{
    [Fact]
    public void Create_UnitOfMeasure_Should_Set_All_Properties()
    {
        // Arrange & Act
        var unit = new UnitOfMeasure(
            Guid.NewGuid(),
            "KG",
            "千克",
            "kg",
            UnitType.Weight.Value,
            true);

        // Assert
        unit.UnitCode.ShouldBe("KG");
        unit.UnitName.ShouldBe("千克");
        unit.UnitSymbol.ShouldBe("kg");
        unit.UnitType.ShouldBe(UnitType.Weight.Value);
        unit.IsActive.ShouldBeTrue();
    }

    [Fact]
    public void SetUnitCode_Should_Update_Code()
    {
        var unit = CreateSampleUnit();
        unit.SetUnitCode("G");
        unit.UnitCode.ShouldBe("G");
    }

    [Fact]
    public void SetUnitCode_Null_Should_Throw()
    {
        var unit = CreateSampleUnit();
        Should.Throw<ArgumentNullException>(() => unit.SetUnitCode(null));
    }

    [Fact]
    public void SetUnitName_Should_Update_Name()
    {
        var unit = CreateSampleUnit();
        unit.SetUnitName("克");
        unit.UnitName.ShouldBe("克");
    }

    [Fact]
    public void SetUnitName_Null_Should_Throw()
    {
        var unit = CreateSampleUnit();
        Should.Throw<ArgumentNullException>(() => unit.SetUnitName(null));
    }

    private static UnitOfMeasure CreateSampleUnit()
    {
        return new UnitOfMeasure(
            Guid.NewGuid(),
            "KG",
            "千克",
            "kg",
            UnitType.Weight.Value,
            true);
    }
}
