using Shouldly;
using Wms.Warehouse.Domain.Aggregates;
using WarehouseEntity = Wms.Warehouse.Domain.Aggregates.Warehouse;
using Wms.Warehouse.Domain.Enums;
using Wms.Warehouse.Domain.Events;

namespace Wms.Warehouse.Tests.Domain;

/// <summary>
/// Warehouse Domain Tests — tests aggregate root creation, code/name/type setting,
/// activation/deactivation, capacity validation, and domain events.
/// (Phase 8 Coding Conventions, Section 6)
/// </summary>
public class WarehouseTests
{
    #region Constructor Tests

    [Fact]
    public void Create_Warehouse_Should_Set_All_Properties()
    {
        // Arrange & Act
        var warehouse = new WarehouseEntity(
            Guid.NewGuid(),
            "WH-001",
            "原材料仓库",
            WarehouseType.RawMaterial.Value,
            "ORG-001",
            "制造事业部",
            "PLANT-001",
            "工厂A",
            StorageConditionType.Normal.Value,
            3,
            true);

        // Assert
        warehouse.WarehouseCode.ShouldBe("WH-001");
        warehouse.WarehouseName.ShouldBe("原材料仓库");
        warehouse.WarehouseType.ShouldBe(WarehouseType.RawMaterial.Value);
        warehouse.OrganizationUnitName.ShouldBe("制造事业部");
        warehouse.PlantName.ShouldBe("工厂A");
        warehouse.StorageConditionType.ShouldBe(StorageConditionType.Normal.Value);
        warehouse.LocationLevelCount.ShouldBe(3);
        warehouse.IsActive.ShouldBeTrue();
    }

    [Fact]
    public void Create_Warehouse_With_Null_OrganizationUnitName_Should_Throw()
    {
        Should.Throw<ArgumentNullException>(() => new WarehouseEntity(
            Guid.NewGuid(), "WH-001", "原材料仓库",
            WarehouseType.RawMaterial.Value,
            "ORG-001", null!,
            "PLANT-001", "工厂A"));
    }

    [Fact]
    public void Create_Warehouse_With_Null_PlantName_Should_Throw()
    {
        Should.Throw<ArgumentNullException>(() => new WarehouseEntity(
            Guid.NewGuid(), "WH-001", "原材料仓库",
            WarehouseType.RawMaterial.Value,
            "ORG-001", "制造事业部",
            "PLANT-001", null!));
    }

    [Fact]
    public void Create_Warehouse_Should_Fire_Created_Event()
    {
        var warehouseId = Guid.NewGuid();
        var warehouse = new WarehouseEntity(
            warehouseId, "WH-001", "原材料仓库",
            WarehouseType.RawMaterial.Value,
            "ORG-001", "制造事业部",
            "PLANT-001", "工厂A");

        var events = warehouse.GetLocalEvents();
        events.ShouldNotBeEmpty();
        events.ShouldContain(e => e is WarehouseCreatedEvent);
        var createdEvent = events.OfType<WarehouseCreatedEvent>().First();
        createdEvent.WarehouseId.ShouldBe(warehouseId);
        createdEvent.WarehouseCode.ShouldBe("WH-001");
    }

    #endregion

    #region SetWarehouseCode Tests

    [Fact]
    public void SetWarehouseCode_Should_Update_Code()
    {
        var warehouse = CreateTestWarehouse();
        warehouse.SetWarehouseCode("WH-002");
        warehouse.WarehouseCode.ShouldBe("WH-002");
    }

    [Fact]
    public void SetWarehouseCode_Empty_Should_Throw()
    {
        var warehouse = CreateTestWarehouse();
        Should.Throw<ArgumentException>(() => warehouse.SetWarehouseCode(""));
    }

    [Fact]
    public void SetWarehouseCode_Whitespace_Should_Throw()
    {
        var warehouse = CreateTestWarehouse();
        Should.Throw<ArgumentException>(() => warehouse.SetWarehouseCode("   "));
    }

    [Fact]
    public void SetWarehouseCode_Should_Trim()
    {
        var warehouse = CreateTestWarehouse();
        warehouse.SetWarehouseCode("  WH-003  ");
        warehouse.WarehouseCode.ShouldBe("WH-003");
    }

    #endregion

    #region SetWarehouseName Tests

    [Fact]
    public void SetWarehouseName_Should_Update_Name()
    {
        var warehouse = CreateTestWarehouse();
        warehouse.SetWarehouseName("成品仓库");
        warehouse.WarehouseName.ShouldBe("成品仓库");
    }

    [Fact]
    public void SetWarehouseName_Empty_Should_Throw()
    {
        var warehouse = CreateTestWarehouse();
        Should.Throw<ArgumentException>(() => warehouse.SetWarehouseName(""));
    }

    #endregion

    #region SetType Tests

    [Fact]
    public void SetType_Should_Update_Type()
    {
        var warehouse = CreateTestWarehouse();
        warehouse.SetType(WarehouseType.Finished.Value);
        warehouse.WarehouseType.ShouldBe(WarehouseType.Finished.Value);
    }

    [Fact]
    public void SetType_Invalid_Should_Throw()
    {
        var warehouse = CreateTestWarehouse();
        Should.Throw<ArgumentException>(() => warehouse.SetType(999));
    }

    [Fact]
    public void SetType_AllWarehouseTypes_Should_Be_Valid()
    {
        var warehouse = CreateTestWarehouse();
        var allTypes = WarehouseType.List;

        foreach (var whType in allTypes)
        {
            Should.NotThrow(() => warehouse.SetType(whType.Value));
            warehouse.WarehouseType.ShouldBe(whType.Value);
        }
    }

    #endregion

    #region SetResponsibleUser Tests

    [Fact]
    public void SetResponsibleUser_Should_Update_User()
    {
        var warehouse = CreateTestWarehouse();
        var userId = "USER-001";
        warehouse.SetResponsibleUser(userId, "张三");
        warehouse.ResponsibleUserId.ShouldBe(userId);
        warehouse.ResponsibleUserName.ShouldBe("张三");
    }

    [Fact]
    public void SetResponsibleUser_Should_Clear_User()
    {
        var warehouse = CreateTestWarehouse();
        var userId = "USER-001";
        warehouse.SetResponsibleUser(userId, "张三");
        warehouse.SetResponsibleUser(null, null);
        warehouse.ResponsibleUserId.ShouldBeNull();
        warehouse.ResponsibleUserName.ShouldBeNull();
    }

    #endregion

    #region SetOrganizationUnitName Tests

    [Fact]
    public void SetOrganizationUnitName_Should_Update_Name()
    {
        var warehouse = CreateTestWarehouse();
        warehouse.SetOrganizationUnitName("新事业部");
        warehouse.OrganizationUnitName.ShouldBe("新事业部");
    }

    [Fact]
    public void SetOrganizationUnitName_Null_Should_Throw()
    {
        var warehouse = CreateTestWarehouse();
        Should.Throw<ArgumentNullException>(() => warehouse.SetOrganizationUnitName(null!));
    }

    #endregion

    #region SetPlantName Tests

    [Fact]
    public void SetPlantName_Should_Update_Name()
    {
        var warehouse = CreateTestWarehouse();
        warehouse.SetPlantName("工厂B");
        warehouse.PlantName.ShouldBe("工厂B");
    }

    [Fact]
    public void SetPlantName_Null_Should_Throw()
    {
        var warehouse = CreateTestWarehouse();
        Should.Throw<ArgumentNullException>(() => warehouse.SetPlantName(null!));
    }

    #endregion

    #region SetAddress Tests

    [Fact]
    public void SetAddress_Should_Update_Address()
    {
        var warehouse = CreateTestWarehouse();
        warehouse.SetAddress("深圳市南山区");
        warehouse.Address.ShouldBe("深圳市南山区");
    }

    [Fact]
    public void SetAddress_Null_Should_Set_Null()
    {
        var warehouse = CreateTestWarehouse();
        warehouse.SetAddress("深圳市南山区");
        warehouse.SetAddress(null);
        warehouse.Address.ShouldBeNull();
    }

    #endregion

    #region SetStorageConditionType Tests

    [Fact]
    public void SetStorageConditionType_Should_Update_Type()
    {
        var warehouse = CreateTestWarehouse();
        warehouse.SetStorageConditionType(StorageConditionType.ColdChain.Value);
        warehouse.StorageConditionType.ShouldBe(StorageConditionType.ColdChain.Value);
    }

    [Fact]
    public void SetStorageConditionType_Invalid_Should_Throw()
    {
        var warehouse = CreateTestWarehouse();
        Should.Throw<ArgumentException>(() => warehouse.SetStorageConditionType(999));
    }

    [Fact]
    public void SetStorageConditionType_AllTypes_Should_Be_Valid()
    {
        var warehouse = CreateTestWarehouse();
        var allTypes = StorageConditionType.List;

        foreach (var scType in allTypes)
        {
            Should.NotThrow(() => warehouse.SetStorageConditionType(scType.Value));
            warehouse.StorageConditionType.ShouldBe(scType.Value);
        }
    }

    #endregion

    #region SetRemark Tests

    [Fact]
    public void SetRemark_Should_Update_Remark()
    {
        var warehouse = CreateTestWarehouse();
        warehouse.SetRemark("这是一条备注");
        warehouse.Remark.ShouldBe("这是一条备注");
    }

    [Fact]
    public void SetRemark_Null_Should_Set_Null()
    {
        var warehouse = CreateTestWarehouse();
        warehouse.SetRemark("备注");
        warehouse.SetRemark(null);
        warehouse.Remark.ShouldBeNull();
    }

    [Fact]
    public void SetRemark_Should_Trim()
    {
        var warehouse = CreateTestWarehouse();
        warehouse.SetRemark("  带空格的备注  ");
        warehouse.Remark.ShouldBe("带空格的备注");
    }

    #endregion

    #region SetLocationLevelCount Tests

    [Fact]
    public void SetLocationLevelCount_Should_Update_Count()
    {
        var warehouse = CreateTestWarehouse();
        warehouse.SetLocationLevelCount(4);
        warehouse.LocationLevelCount.ShouldBe(4);
    }

    [Fact]
    public void SetLocationLevelCount_Invalid_Should_Throw()
    {
        var warehouse = CreateTestWarehouse();
        Should.Throw<ArgumentException>(() => warehouse.SetLocationLevelCount(5));
    }

    [Fact]
    public void SetLocationLevelCount_Too_Small_Should_Throw()
    {
        var warehouse = CreateTestWarehouse();
        Should.Throw<ArgumentException>(() => warehouse.SetLocationLevelCount(2));
    }

    [Fact]
    public void SetLocationLevelCount_Zero_Should_Throw()
    {
        var warehouse = CreateTestWarehouse();
        Should.Throw<ArgumentException>(() => warehouse.SetLocationLevelCount(0));
    }

    [Fact]
    public void SetLocationLevelCount_Boundary_3_Should_Succeed()
    {
        var warehouse = CreateTestWarehouse();
        warehouse.SetLocationLevelCount(4);
        warehouse.SetLocationLevelCount(3);
        warehouse.LocationLevelCount.ShouldBe(3);
    }

    [Fact]
    public void SetLocationLevelCount_Boundary_4_Should_Succeed()
    {
        var warehouse = CreateTestWarehouse();
        warehouse.SetLocationLevelCount(4);
        warehouse.LocationLevelCount.ShouldBe(4);
    }

    #endregion

    #region SetActive / Deactivate Tests

    [Fact]
    public void SetActive_Should_Set_IsActive_True()
    {
        var warehouse = CreateTestWarehouse();
        warehouse.Deactivate();
        warehouse.IsActive.ShouldBeFalse();
        warehouse.SetActive();
        warehouse.IsActive.ShouldBeTrue();
    }

    [Fact]
    public void Deactivate_Should_Set_IsActive_False()
    {
        var warehouse = CreateTestWarehouse();
        warehouse.IsActive.ShouldBeTrue();
        warehouse.Deactivate();
        warehouse.IsActive.ShouldBeFalse();
    }

    [Fact]
    public void Deactivate_Should_Fire_Deactivated_Event()
    {
        var warehouse = CreateTestWarehouse();
        warehouse.Deactivate();

        var events = warehouse.GetLocalEvents();
        events.ShouldContain(e => e is WarehouseDeactivatedEvent);
        var deactivatedEvent = events.OfType<WarehouseDeactivatedEvent>().First();
        deactivatedEvent.WarehouseId.ShouldBe(warehouse.Id);
        deactivatedEvent.WarehouseCode.ShouldBe(warehouse.WarehouseCode);
    }

    #endregion

    #region ValidateCapacity Tests

    [Fact]
    public void ValidateCapacity_ActiveWarehouse_Should_ReturnTrue()
    {
        var warehouse = CreateTestWarehouse();
        warehouse.ValidateCapacity().ShouldBeTrue();
    }

    [Fact]
    public void ValidateCapacity_DeactivatedWarehouse_Should_ReturnFalse()
    {
        var warehouse = CreateTestWarehouse();
        warehouse.Deactivate();
        warehouse.ValidateCapacity().ShouldBeFalse();
    }

    [Fact]
    public void ValidateCapacity_LevelCount_3_Should_ReturnTrue()
    {
        var warehouse = CreateTestWarehouse();
        warehouse.SetLocationLevelCount(3);
        warehouse.ValidateCapacity().ShouldBeTrue();
    }

    [Fact]
    public void ValidateCapacity_LevelCount_4_Should_ReturnTrue()
    {
        var warehouse = CreateTestWarehouse();
        warehouse.SetLocationLevelCount(4);
        warehouse.ValidateCapacity().ShouldBeTrue();
    }

    #endregion

    #region SmartEnum Coverage Tests

    [Fact]
    public void WarehouseType_FromValue_Should_Return_Correct_Type()
    {
        WarehouseType.FromValue(0).ShouldBe(WarehouseType.RawMaterial);
        WarehouseType.FromValue(1).ShouldBe(WarehouseType.Finished);
        WarehouseType.FromValue(2).ShouldBe(WarehouseType.LineSide);
    }

    [Fact]
    public void WarehouseType_FromName_Should_Return_Correct_Type()
    {
        WarehouseType.FromName("RawMaterial").ShouldBe(WarehouseType.RawMaterial);
        WarehouseType.FromName("Finished").ShouldBe(WarehouseType.Finished);
    }

    [Fact]
    public void WarehouseType_Should_Have_12_Types()
    {
        WarehouseType.List.Count.ShouldBe(12);
    }

    #endregion

    #region Private Helpers

    private static WarehouseEntity CreateTestWarehouse()
    {
        return new WarehouseEntity(
            Guid.NewGuid(),
            "WH-001",
            "原材料仓库",
            WarehouseType.RawMaterial.Value,
            "ORG-001",
            "制造事业部",
            "PLANT-001",
            "工厂A");
    }

    #endregion
}
