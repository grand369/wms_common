using Shouldly;
using Wms.Material.Domain.Aggregates;
using MaterialEntity = Wms.Material.Domain.Aggregates.Material;
using Wms.Material.Domain.Enums;
using Wms.Material.Domain.ValueObjects;

namespace Wms.Material.Tests.Domain;

/// <summary>
/// Material Domain Tests — tests aggregate root creation, code/name/type setting, value object updates,
/// substitute relation management, and activation/deactivation.
/// (Phase 8 Coding Conventions, Section 6)
/// </summary>
public class MaterialTests
{
    [Fact]
    public void Create_Material_Should_Set_All_Properties()
    {
        // Arrange & Act
        var unitId = Guid.NewGuid();
        var material = new MaterialEntity(
            Guid.NewGuid(),
            "MT-001",
            "钢板Q235B",
            MaterialType.RawMaterial.Value,
            unitId,
            "千克",
            new StorageAttribute(0, 5, "标准包装", 25.5m),
            new QualityAttribute(true, false, false, 0, QualityInspectionMode.SamplingInspection.Value),
            new InventoryAttribute(100, 50, ABCClassificationType.A.Value, false),
            new IssueStrategy(IssueStrategyType.FIFO.Value, StrategyScope.ByMaterial.Value),
            ErpSyncStatus.None.Value,
            true);

        // Assert
        material.MaterialCode.ShouldBe("MT-001");
        material.MaterialName.ShouldBe("钢板Q235B");
        material.MaterialType.ShouldBe(MaterialType.RawMaterial.Value);
        material.PrimaryUnitId.ShouldBe(unitId);
        material.PrimaryUnitName.ShouldBe("千克");
        material.StorageAttribute.StorageConditionType.ShouldBe(0);
        material.StorageAttribute.MaxStackingLayers.ShouldBe(5);
        material.QualityAttribute.BatchManagementEnabled.ShouldBeTrue();
        material.InventoryAttribute.SafetyStockQuantity.ShouldBe(100);
        material.IssueStrategy.IssueStrategyType.ShouldBe(IssueStrategyType.FIFO.Value);
        material.ErpSyncStatus.ShouldBe(ErpSyncStatus.None.Value);
        material.IsActive.ShouldBeTrue();
    }

    [Fact]
    public void SetMaterialCode_Should_Update_Code()
    {
        var material = CreateSampleMaterial();
        material.SetMaterialCode("MT-002");
        material.MaterialCode.ShouldBe("MT-002");
    }

    [Fact]
    public void SetMaterialCode_Empty_Should_Throw()
    {
        var material = CreateSampleMaterial();
        Should.Throw<ArgumentException>(() => material.SetMaterialCode(""));
    }

    [Fact]
    public void SetMaterialName_Should_Update_Name()
    {
        var material = CreateSampleMaterial();
        material.SetMaterialName("铝板6061");
        material.MaterialName.ShouldBe("铝板6061");
    }

    [Fact]
    public void SetMaterialName_Empty_Should_Throw()
    {
        var material = CreateSampleMaterial();
        Should.Throw<ArgumentException>(() => material.SetMaterialName(""));
    }

    [Fact]
    public void SetType_Should_Update_Type()
    {
        var material = CreateSampleMaterial();
        material.SetType(MaterialType.Finished.Value);
        material.MaterialType.ShouldBe(MaterialType.Finished.Value);
    }

    [Fact]
    public void SetType_Invalid_Should_Throw()
    {
        var material = CreateSampleMaterial();
        Should.Throw<ArgumentException>(() => material.SetType(999));
    }

    [Fact]
    public void UpdateStorageAttribute_Should_Update_ValueObject()
    {
        var material = CreateSampleMaterial();
        var newStorageAttr = new StorageAttribute(2, 3, "冷藏包装", 30.0m);
        material.UpdateStorageAttribute(newStorageAttr);
        material.StorageAttribute.StorageConditionType.ShouldBe(2);
        material.StorageAttribute.MaxStackingLayers.ShouldBe(3);
    }

    [Fact]
    public void UpdateStorageAttribute_Null_Should_Throw()
    {
        var material = CreateSampleMaterial();
        Should.Throw<ArgumentNullException>(() => material.UpdateStorageAttribute(null));
    }

    [Fact]
    public void UpdateQualityAttribute_Should_Update_ValueObject()
    {
        var material = CreateSampleMaterial();
        var newQualityAttr = new QualityAttribute(true, true, true, 365, QualityInspectionMode.FullInspection.Value);
        material.UpdateQualityAttribute(newQualityAttr);
        material.QualityAttribute.BatchManagementEnabled.ShouldBeTrue();
        material.QualityAttribute.SerialManagementEnabled.ShouldBeTrue();
        material.QualityAttribute.ExpiryManagementEnabled.ShouldBeTrue();
        material.QualityAttribute.ShelfLifeDays.ShouldBe(365);
    }

    [Fact]
    public void UpdateInventoryAttribute_Should_Update_ValueObject()
    {
        var material = CreateSampleMaterial();
        var newInventoryAttr = new InventoryAttribute(200, 100, ABCClassificationType.A.Value, true);
        material.UpdateInventoryAttribute(newInventoryAttr);
        material.InventoryAttribute.SafetyStockQuantity.ShouldBe(200);
        material.InventoryAttribute.AllowNegativeInventory.ShouldBeTrue();
    }

    [Fact]
    public void UpdateIssueStrategy_Should_Update_ValueObject()
    {
        var material = CreateSampleMaterial();
        var newIssueStrategy = new IssueStrategy(IssueStrategyType.FEFO.Value, StrategyScope.ByWarehouse.Value);
        material.UpdateIssueStrategy(newIssueStrategy);
        material.IssueStrategy.IssueStrategyType.ShouldBe(IssueStrategyType.FEFO.Value);
        material.IssueStrategy.StrategyScope.ShouldBe(StrategyScope.ByWarehouse.Value);
    }

    [Fact]
    public void UpdateDangerAttribute_Should_Set_DangerAttribute()
    {
        var material = CreateSampleMaterial();
        var dangerAttr = new DangerAttribute(DangerLevelType.High.Value, "MSDS-001", "易燃");
        material.UpdateDangerAttribute(dangerAttr);
        material.DangerAttribute.ShouldNotBeNull();
        material.DangerAttribute.DangerLevel.ShouldBe(DangerLevelType.High.Value);
        material.DangerAttribute.MSDSNumber.ShouldBe("MSDS-001");
    }

    [Fact]
    public void UpdateDangerAttribute_Null_Should_Clear_DangerAttribute()
    {
        var material = CreateSampleMaterial();
        var dangerAttr = new DangerAttribute(DangerLevelType.High.Value, "MSDS-001", "易燃");
        material.UpdateDangerAttribute(dangerAttr);
        material.DangerAttribute.ShouldNotBeNull();

        material.UpdateDangerAttribute(null);
        material.DangerAttribute.ShouldBeNull();
    }

    [Fact]
    public void AddSubstituteRelation_Should_Add_To_List()
    {
        var material = CreateSampleMaterial();
        var substituteId = Guid.NewGuid();
        material.AddSubstituteRelation(substituteId, "MT-SUB01", 1, 1.0m);

        material.SubstituteRelations.Count.ShouldBe(1);
        material.SubstituteRelations[0].SubstituteMaterialId.ShouldBe(substituteId);
        material.SubstituteRelations[0].SubstituteMaterialCode.ShouldBe("MT-SUB01");
        material.SubstituteRelations[0].SubstitutePriority.ShouldBe(1);
        material.SubstituteRelations[0].SubstituteRatio.ShouldBe(1.0m);
    }

    [Fact]
    public void AddSubstituteRelation_Duplicate_Should_Throw()
    {
        var material = CreateSampleMaterial();
        var substituteId = Guid.NewGuid();
        material.AddSubstituteRelation(substituteId, "MT-SUB01", 1, 1.0m);

        Should.Throw<ArgumentException>(() => material.AddSubstituteRelation(substituteId, "MT-SUB01", 2, 1.5m));
    }

    [Fact]
    public void RemoveSubstituteRelation_Should_Remove_From_List()
    {
        var material = CreateSampleMaterial();
        var substituteId = Guid.NewGuid();
        material.AddSubstituteRelation(substituteId, "MT-SUB01", 1, 1.0m);

        var relationId = material.SubstituteRelations[0].Id;
        material.RemoveSubstituteRelation(relationId);

        material.SubstituteRelations.Count.ShouldBe(0);
    }

    [Fact]
    public void RemoveSubstituteRelation_NotFound_Should_Throw()
    {
        var material = CreateSampleMaterial();
        Should.Throw<ArgumentException>(() => material.RemoveSubstituteRelation(Guid.NewGuid()));
    }

    [Fact]
    public void SetActive_Should_Set_IsActive_True()
    {
        var material = CreateSampleMaterial();
        material.Deactivate();
        material.IsActive.ShouldBeFalse();
        material.SetActive();
        material.IsActive.ShouldBeTrue();
    }

    [Fact]
    public void Deactivate_Should_Set_IsActive_False()
    {
        var material = CreateSampleMaterial();
        material.IsActive.ShouldBeTrue();
        material.Deactivate();
        material.IsActive.ShouldBeFalse();
    }

    private static MaterialEntity CreateSampleMaterial()
    {
        return new MaterialEntity(
            Guid.NewGuid(),
            "MT-001",
            "钢板Q235B",
            MaterialType.RawMaterial.Value,
            Guid.NewGuid(),
            "千克",
            new StorageAttribute(0, 5, "标准包装", 25.5m),
            new QualityAttribute(true, false, false, 0, QualityInspectionMode.SamplingInspection.Value),
            new InventoryAttribute(100, 50, ABCClassificationType.A.Value, false),
            new IssueStrategy(IssueStrategyType.FIFO.Value, StrategyScope.ByMaterial.Value),
            ErpSyncStatus.None.Value,
            true);
    }
}
