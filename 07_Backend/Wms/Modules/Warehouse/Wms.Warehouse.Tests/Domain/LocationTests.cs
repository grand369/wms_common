using Shouldly;
using Wms.Warehouse.Domain.Aggregates;
using Wms.Warehouse.Domain.Enums;
using Wms.Warehouse.Domain.Events;

namespace Wms.Warehouse.Tests.Domain;

/// <summary>
/// Location Domain Tests — tests aggregate root creation, code setting,
/// putaway compatibility validation, capacity/weight management,
/// activation/deactivation, and domain events.
/// (Phase 8 Coding Conventions, Section 6)
/// </summary>
public class LocationTests
{
    #region Constructor Tests

    [Fact]
    public void Create_Location_Should_Set_All_Properties()
    {
        var warehouseId = "WH-ID-001";
        var areaId = "AREA-ID-001";
        var location = new Location(
            Guid.NewGuid(),
            "LOC-001",
            warehouseId,
            "WH-001",
            areaId,
            "A-01",
            "BAR-LOC001",
            LocationType.Standard.Value,
            StorageConditionType.Normal.Value,
            500,
            100,
            "R01",
            "C01",
            "L01",
            true);

        location.LocationCode.ShouldBe("LOC-001");
        location.WarehouseId.ShouldBe(warehouseId);
        location.WarehouseCode.ShouldBe("WH-001");
        location.AreaId.ShouldBe(areaId);
        location.AreaCode.ShouldBe("A-01");
        location.BarcodeId.ShouldBe("BAR-LOC001");
        location.LocationType.ShouldBe(LocationType.Standard.Value);
        location.StorageCondition.ShouldBe(StorageConditionType.Normal.Value);
        location.MaxWeight.ShouldBe(500);
        location.MaxCapacity.ShouldBe(100);
        location.Row.ShouldBe("R01");
        location.Column.ShouldBe("C01");
        location.Layer.ShouldBe("L01");
        location.IsActive.ShouldBeTrue();
        location.CurrentWeight.ShouldBe(0);
        location.CurrentCapacity.ShouldBe(0);
    }

    [Fact]
    public void Create_Location_Should_Fire_Created_Event()
    {
        var locationId = Guid.NewGuid();
        var warehouseId = "WH-ID-001";
        var areaId = "AREA-ID-001";

        var location = new Location(
            locationId, "LOC-001", warehouseId, "WH-001",
            areaId, "A-01", "BAR-LOC001",
            LocationType.Standard.Value, StorageConditionType.Normal.Value);

        var events = location.GetLocalEvents();
        events.ShouldContain(e => e is LocationCreatedEvent);
        var createdEvent = events.OfType<LocationCreatedEvent>().First();
        createdEvent.LocationId.ShouldBe(locationId);
        createdEvent.LocationCode.ShouldBe("LOC-001");
        createdEvent.WarehouseId.ShouldBe(warehouseId);
        createdEvent.AreaId.ShouldBe(areaId);
    }

    #endregion

    #region SetLocationCode Tests

    [Fact]
    public void SetLocationCode_Should_Update_Code()
    {
        var location = CreateTestLocation();
        location.SetLocationCode("LOC-002");
        location.LocationCode.ShouldBe("LOC-002");
    }

    [Fact]
    public void SetLocationCode_Empty_Should_Throw()
    {
        var location = CreateTestLocation();
        Should.Throw<ArgumentException>(() => location.SetLocationCode(""));
    }

    [Fact]
    public void SetLocationCode_Whitespace_Should_Throw()
    {
        var location = CreateTestLocation();
        Should.Throw<ArgumentException>(() => location.SetLocationCode("   "));
    }

    #endregion

    #region SetLocationType Tests

    [Fact]
    public void SetLocationType_Should_Update_Type()
    {
        var location = CreateTestLocation();
        location.SetLocationType(LocationType.Shelf.Value);
        location.LocationType.ShouldBe(LocationType.Shelf.Value);
    }

    [Fact]
    public void SetLocationType_Invalid_Should_Throw()
    {
        var location = CreateTestLocation();
        Should.Throw<ArgumentException>(() => location.SetLocationType(999));
    }

    [Fact]
    public void SetLocationType_AllTypes_Should_Be_Valid()
    {
        var location = CreateTestLocation();
        var allTypes = LocationType.List;

        foreach (var locType in allTypes)
        {
            Should.NotThrow(() => location.SetLocationType(locType.Value));
            location.LocationType.ShouldBe(locType.Value);
        }
    }

    #endregion

    #region SetStorageCondition Tests

    [Fact]
    public void SetStorageCondition_Should_Update_Condition()
    {
        var location = CreateTestLocation();
        location.SetStorageCondition(StorageConditionType.ColdChain.Value);
        location.StorageCondition.ShouldBe(StorageConditionType.ColdChain.Value);
    }

    [Fact]
    public void SetStorageCondition_Invalid_Should_Throw()
    {
        var location = CreateTestLocation();
        Should.Throw<ArgumentException>(() => location.SetStorageCondition(999));
    }

    [Fact]
    public void SetStorageCondition_AllTypes_Should_Be_Valid()
    {
        var location = CreateTestLocation();
        var allTypes = StorageConditionType.List;

        foreach (var scType in allTypes)
        {
            Should.NotThrow(() => location.SetStorageCondition(scType.Value));
            location.StorageCondition.ShouldBe(scType.Value);
        }
    }

    #endregion

    #region SetMaxWeight Tests

    [Fact]
    public void SetMaxWeight_Should_Update_Weight()
    {
        var location = CreateTestLocation();
        location.SetMaxWeight(1000);
        location.MaxWeight.ShouldBe(1000);
    }

    [Fact]
    public void SetMaxWeight_Null_Should_Set_Null()
    {
        var location = CreateTestLocation();
        location.SetMaxWeight(500);
        location.SetMaxWeight(null);
        location.MaxWeight.ShouldBeNull();
    }

    #endregion

    #region SetMaxCapacity Tests

    [Fact]
    public void SetMaxCapacity_Should_Update_Capacity()
    {
        var location = CreateTestLocation();
        location.SetMaxCapacity(200);
        location.MaxCapacity.ShouldBe(200);
    }

    [Fact]
    public void SetMaxCapacity_Null_Should_Set_Null()
    {
        var location = CreateTestLocation();
        location.SetMaxCapacity(100);
        location.SetMaxCapacity(null);
        location.MaxCapacity.ShouldBeNull();
    }

    #endregion

    #region SetCoordinates Tests

    [Fact]
    public void SetCoordinates_Should_Update_Coordinates()
    {
        var location = CreateTestLocation();
        location.SetCoordinates("R02", "C02", "L02");
        location.Row.ShouldBe("R02");
        location.Column.ShouldBe("C02");
        location.Layer.ShouldBe("L02");
    }

    [Fact]
    public void SetCoordinates_Partial_Null_Should_Succeed()
    {
        var location = CreateTestLocation();
        location.SetCoordinates(null, "C03", null);
        location.Row.ShouldBeNull();
        location.Column.ShouldBe("C03");
        location.Layer.ShouldBeNull();
    }

    #endregion

    #region ValidatePutawayCompatibility Tests

    [Fact]
    public void ValidatePutawayCompatibility_NormalToNormal_Should_ReturnTrue()
    {
        var location = CreateTestLocation(StorageConditionType.Normal.Value);
        location.ValidatePutawayCompatibility(StorageConditionType.Normal.Value).ShouldBeTrue();
    }

    [Fact]
    public void ValidatePutawayCompatibility_NormalToColdChain_Should_ReturnFalse()
    {
        var location = CreateTestLocation(StorageConditionType.Normal.Value);
        location.ValidatePutawayCompatibility(StorageConditionType.ColdChain.Value).ShouldBeFalse();
    }

    [Fact]
    public void ValidatePutawayCompatibility_NormalToConstantTemp_Should_ReturnFalse()
    {
        var location = CreateTestLocation(StorageConditionType.Normal.Value);
        location.ValidatePutawayCompatibility(StorageConditionType.ConstantTemp.Value).ShouldBeFalse();
    }

    [Fact]
    public void ValidatePutawayCompatibility_NormalToMoistureProof_Should_ReturnFalse()
    {
        var location = CreateTestLocation(StorageConditionType.Normal.Value);
        location.ValidatePutawayCompatibility(StorageConditionType.MoistureProof.Value).ShouldBeFalse();
    }

    [Fact]
    public void ValidatePutawayCompatibility_NormalToDustProof_Should_ReturnFalse()
    {
        var location = CreateTestLocation(StorageConditionType.Normal.Value);
        location.ValidatePutawayCompatibility(StorageConditionType.DustProof.Value).ShouldBeFalse();
    }

    [Fact]
    public void ValidatePutawayCompatibility_ColdChainToColdChain_Should_ReturnTrue()
    {
        var location = CreateTestLocation(StorageConditionType.ColdChain.Value);
        location.ValidatePutawayCompatibility(StorageConditionType.ColdChain.Value).ShouldBeTrue();
    }

    [Fact]
    public void ValidatePutawayCompatibility_ColdChainToNormal_Should_ReturnTrue()
    {
        var location = CreateTestLocation(StorageConditionType.ColdChain.Value);
        location.ValidatePutawayCompatibility(StorageConditionType.Normal.Value).ShouldBeTrue();
    }

    [Fact]
    public void ValidatePutawayCompatibility_ColdChainToConstantTemp_Should_ReturnFalse()
    {
        var location = CreateTestLocation(StorageConditionType.ColdChain.Value);
        // Cross-specialization is not allowed
        location.ValidatePutawayCompatibility(StorageConditionType.ConstantTemp.Value).ShouldBeFalse();
    }

    [Fact]
    public void ValidatePutawayCompatibility_ConstantTempToNormal_Should_ReturnTrue()
    {
        var location = CreateTestLocation(StorageConditionType.ConstantTemp.Value);
        location.ValidatePutawayCompatibility(StorageConditionType.Normal.Value).ShouldBeTrue();
    }

    [Fact]
    public void ValidatePutawayCompatibility_MoistureProofToNormal_Should_ReturnTrue()
    {
        var location = CreateTestLocation(StorageConditionType.MoistureProof.Value);
        location.ValidatePutawayCompatibility(StorageConditionType.Normal.Value).ShouldBeTrue();
    }

    [Fact]
    public void ValidatePutawayCompatibility_DustProofToNormal_Should_ReturnTrue()
    {
        var location = CreateTestLocation(StorageConditionType.DustProof.Value);
        location.ValidatePutawayCompatibility(StorageConditionType.Normal.Value).ShouldBeTrue();
    }

    [Fact]
    public void ValidatePutawayCompatibility_Deactivated_Should_ReturnFalse()
    {
        var location = CreateTestLocation(StorageConditionType.Normal.Value);
        location.Deactivate();
        location.ValidatePutawayCompatibility(StorageConditionType.Normal.Value).ShouldBeFalse();
    }

    #endregion

    #region UpdateCurrentWeight Tests

    [Fact]
    public void UpdateCurrentWeight_Should_Update_Weight()
    {
        var location = CreateTestLocation();
        location.UpdateCurrentWeight(250);
        location.CurrentWeight.ShouldBe(250);
    }

    [Fact]
    public void UpdateCurrentWeight_ExceedsMax_Should_Throw()
    {
        var location = CreateTestLocation();
        location.SetMaxWeight(500);
        Should.Throw<ArgumentException>(() => location.UpdateCurrentWeight(600));
    }

    [Fact]
    public void UpdateCurrentWeight_Boundary_At_Max_Should_Succeed()
    {
        var location = CreateTestLocation();
        location.SetMaxWeight(500);
        location.UpdateCurrentWeight(500);
        location.CurrentWeight.ShouldBe(500);
    }

    [Fact]
    public void UpdateCurrentWeight_Null_Max_Should_Always_Succeed()
    {
        var location = CreateTestLocation();
        location.SetMaxWeight(null);
        Should.NotThrow(() => location.UpdateCurrentWeight(9999));
    }

    #endregion

    #region UpdateCurrentCapacity Tests

    [Fact]
    public void UpdateCurrentCapacity_Should_Update_Capacity()
    {
        var location = CreateTestLocation();
        location.UpdateCurrentCapacity(50);
        location.CurrentCapacity.ShouldBe(50);
    }

    [Fact]
    public void UpdateCurrentCapacity_ExceedsMax_Should_Throw()
    {
        var location = CreateTestLocation();
        location.SetMaxCapacity(100);
        Should.Throw<ArgumentException>(() => location.UpdateCurrentCapacity(150));
    }

    [Fact]
    public void UpdateCurrentCapacity_Boundary_At_Max_Should_Succeed()
    {
        var location = CreateTestLocation();
        location.SetMaxCapacity(100);
        location.UpdateCurrentCapacity(100);
        location.CurrentCapacity.ShouldBe(100);
    }

    [Fact]
    public void UpdateCurrentCapacity_Null_Max_Should_Always_Succeed()
    {
        var location = CreateTestLocation();
        location.SetMaxCapacity(null);
        Should.NotThrow(() => location.UpdateCurrentCapacity(9999));
    }

    #endregion

    #region SetActive / Deactivate Tests

    [Fact]
    public void SetActive_Should_Set_IsActive_True()
    {
        var location = CreateTestLocation();
        location.Deactivate();
        location.IsActive.ShouldBeFalse();
        location.SetActive();
        location.IsActive.ShouldBeTrue();
    }

    [Fact]
    public void Deactivate_Should_Set_IsActive_False()
    {
        var location = CreateTestLocation();
        location.Deactivate();
        location.IsActive.ShouldBeFalse();
    }

    [Fact]
    public void SetActive_Should_Fire_StatusChanged_Event()
    {
        var location = CreateTestLocation();
        location.Deactivate();

        var events = location.GetLocalEvents();
        events.ShouldContain(e => e is LocationStatusChangedEvent);
        var statusEvent = events.OfType<LocationStatusChangedEvent>().Last();
        statusEvent.LocationId.ShouldBe(location.Id);
        statusEvent.LocationCode.ShouldBe(location.LocationCode);
        statusEvent.IsActive.ShouldBeFalse();
    }

    [Fact]
    public void Deactivate_Deactivated_Again_Should_Still_Be_False()
    {
        var location = CreateTestLocation();
        location.Deactivate();
        location.Deactivate();
        location.IsActive.ShouldBeFalse();
    }

    #endregion

    #region SmartEnum Coverage Tests

    [Fact]
    public void LocationType_FromValue_Should_Return_Correct_Type()
    {
        LocationType.FromValue(0).ShouldBe(LocationType.Standard);
        LocationType.FromValue(1).ShouldBe(LocationType.Shelf);
        LocationType.FromValue(2).ShouldBe(LocationType.Grid);
    }

    [Fact]
    public void LocationType_FromName_Should_Return_Correct_Type()
    {
        LocationType.FromName("Standard").ShouldBe(LocationType.Standard);
        LocationType.FromName("Shelf").ShouldBe(LocationType.Shelf);
    }

    [Fact]
    public void LocationType_Should_Have_5_Types()
    {
        LocationType.List.Count.ShouldBe(5);
    }

    [Fact]
    public void StorageConditionType_FromValue_Should_Return_Correct_Type()
    {
        StorageConditionType.FromValue(0).ShouldBe(StorageConditionType.Normal);
        StorageConditionType.FromValue(1).ShouldBe(StorageConditionType.ColdChain);
        StorageConditionType.FromValue(2).ShouldBe(StorageConditionType.ConstantTemp);
    }

    [Fact]
    public void StorageConditionType_Should_Have_5_Types()
    {
        StorageConditionType.List.Count.ShouldBe(5);
    }

    #endregion

    #region Private Helpers

    private static Location CreateTestLocation(int storageCondition = 0)
    {
        return new Location(
            Guid.NewGuid(),
            "LOC-001",
            "WH-ID-001",
            "WH-001",
            "AREA-ID-001",
            "A-01",
            "BAR-LOC001",
            LocationType.Standard.Value,
            storageCondition,
            500,
            100);
    }

    #endregion
}
