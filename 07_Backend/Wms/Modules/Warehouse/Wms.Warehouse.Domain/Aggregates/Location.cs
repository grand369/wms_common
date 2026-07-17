using Volo.Abp.Domain.Entities.Auditing;
using Wms.Warehouse.Domain.Enums;
using Wms.Warehouse.Domain.Events;

namespace Wms.Warehouse.Domain.Aggregates;

/// <summary>
/// Location Aggregate Root (AGG-03, ENT-03) — represents the most granular storage unit.
/// Inherits FullAuditedAggregateRoot for ABP audit fields and soft delete.
/// References Warehouse and WarehouseArea via ID + redundant Code (cross-aggregate reference pattern).
/// Has unique barcode for PDA scanning operations.
/// (Phase 3 DDD Design)
/// </summary>
public class Location : FullAuditedAggregateRoot<Guid>
{
    /// <summary>库位编码（唯一条码，用于扫码操作）</summary>
    public string LocationCode { get; private set; } = string.Empty;

    /// <summary>所属仓库ID</summary>
    public string WarehouseId { get; private set; } = string.Empty;

    /// <summary>所属仓库编码（冗余）</summary>
    public string WarehouseCode { get; private set; } = string.Empty;

    /// <summary>所属库区ID</summary>
    public string AreaId { get; private set; } = string.Empty;

    /// <summary>所属库区编码（冗余）</summary>
    public string AreaCode { get; private set; } = string.Empty;

    /// <summary>库位类型枚举值</summary>
    public int LocationType { get; private set; }

    /// <summary>最大承重(kg)</summary>
    public decimal? MaxWeight { get; private set; }

    /// <summary>最大容量</summary>
    public decimal? MaxCapacity { get; private set; }

    /// <summary>当前承重(kg)</summary>
    public decimal? CurrentWeight { get; private set; }

    /// <summary>当前容量</summary>
    public decimal? CurrentCapacity { get; private set; }

    /// <summary>存储条件枚举值（上架兼容性校验）</summary>
    public int StorageCondition { get; private set; }

    /// <summary>条码标识（支持扫码定位）</summary>
    public string BarcodeId { get; private set; } = string.Empty;

    /// <summary>行号</summary>
    public string? Row { get; private set; }

    /// <summary>列号</summary>
    public string? Column { get; private set; }

    /// <summary>层号</summary>
    public string? Layer { get; private set; }

    /// <summary>是否启用</summary>
    public bool IsActive { get; private set; } = true;

    /// <summary>
    /// Creates a new Location aggregate root.
    /// </summary>
    public Location(
        Guid id,
        string locationCode,
        string warehouseId,
        string warehouseCode,
        string areaId,
        string areaCode,
        string barcodeId,
        int locationType = 0,
        int storageCondition = 0,
        decimal? maxWeight = null,
        decimal? maxCapacity = null,
        string? row = null,
        string? column = null,
        string? layer = null,
        bool isActive = true) : base(id)
    {
        SetLocationCode(locationCode);
        WarehouseId = warehouseId ?? throw new ArgumentNullException(nameof(warehouseId));
        WarehouseCode = warehouseCode ?? throw new ArgumentNullException(nameof(warehouseCode));
        AreaId = areaId ?? throw new ArgumentNullException(nameof(areaId));
        AreaCode = areaCode ?? throw new ArgumentNullException(nameof(areaCode));
        BarcodeId = barcodeId ?? throw new ArgumentNullException(nameof(barcodeId));
        LocationType = locationType;
        StorageCondition = storageCondition;
        MaxWeight = maxWeight;
        MaxCapacity = maxCapacity;
        CurrentWeight = 0;
        CurrentCapacity = 0;
        Row = row;
        Column = column;
        Layer = layer;
        IsActive = isActive;

        AddLocalEvent(new LocationCreatedEvent
        {
            LocationId = Id,
            LocationCode = LocationCode,
            WarehouseId = WarehouseId,
            AreaId = AreaId
        });
    }

    /// <summary>
    /// Sets the location code. Must be non-empty and unique.
    /// </summary>
    public Location SetLocationCode(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("Location code cannot be empty.", nameof(code));
        LocationCode = code.Trim();
        return this;
    }

    /// <summary>
    /// Sets the location type.
    /// </summary>
    public Location SetLocationType(int locationType)
    {
        if (!Wms.Warehouse.Domain.Enums.LocationType.TryFromValue(locationType, out _))
            throw new ArgumentException($"Invalid location type value: {locationType}", nameof(locationType));
        LocationType = locationType;
        return this;
    }

    /// <summary>
    /// Sets the storage condition.
    /// </summary>
    public Location SetStorageCondition(int storageCondition)
    {
        if (!StorageConditionType.TryFromValue(storageCondition, out _))
            throw new ArgumentException($"Invalid storage condition type value: {storageCondition}", nameof(storageCondition));
        StorageCondition = storageCondition;
        return this;
    }

    /// <summary>
    /// Sets the max weight.
    /// </summary>
    public Location SetMaxWeight(decimal? maxWeight)
    {
        MaxWeight = maxWeight;
        return this;
    }

    /// <summary>
    /// Sets the max capacity.
    /// </summary>
    public Location SetMaxCapacity(decimal? maxCapacity)
    {
        MaxCapacity = maxCapacity;
        return this;
    }

    /// <summary>
    /// Sets the row/column/layer coordinates.
    /// </summary>
    public Location SetCoordinates(string? row, string? column, string? layer)
    {
        Row = row?.Trim();
        Column = column?.Trim();
        Layer = layer?.Trim();
        return this;
    }

    /// <summary>
    /// Validates whether this location is compatible for putaway given the material's storage condition.
    /// The material's storage condition must be compatible with this location's storage condition.
    /// </summary>
    public bool ValidatePutawayCompatibility(int materialStorageCondition)
    {
        // ColdChain locations can only store ColdChain materials
        // ConstantTemp locations can store ConstantTemp or Normal materials
        // MoistureProof locations can store MoistureProof or Normal materials
        // DustProof locations can store DustProof or Normal materials
        // Normal locations can only store Normal materials
        if (!IsActive)
            return false;

        var locCondition = StorageConditionType.FromValue(StorageCondition);
        var matCondition = StorageConditionType.FromValue(materialStorageCondition);

        // If location condition matches material condition, always compatible
        if (locCondition == matCondition)
            return true;

        // Normal materials can be stored in any specialized location
        if (matCondition == StorageConditionType.Normal)
            return true;

        // Specialized materials cannot be stored in Normal locations
        if (locCondition == StorageConditionType.Normal && matCondition != StorageConditionType.Normal)
            return false;

        // Cross-specialization is not allowed (e.g., ColdChain material in MoistureProof location)
        return false;
    }

    /// <summary>
    /// Updates the current weight of the location (after putaway/pick operations).
    /// </summary>
    public Location UpdateCurrentWeight(decimal currentWeight)
    {
        if (MaxWeight != null && currentWeight > MaxWeight.Value)
            throw new ArgumentException($"Current weight {currentWeight} exceeds max weight {MaxWeight}.");
        CurrentWeight = currentWeight;
        return this;
    }

    /// <summary>
    /// Updates the current capacity of the location.
    /// </summary>
    public Location UpdateCurrentCapacity(decimal currentCapacity)
    {
        if (MaxCapacity != null && currentCapacity > MaxCapacity.Value)
            throw new ArgumentException($"Current capacity {currentCapacity} exceeds max capacity {MaxCapacity}.");
        CurrentCapacity = currentCapacity;
        return this;
    }

    /// <summary>
    /// Activates the location.
    /// </summary>
    public Location SetActive()
    {
        IsActive = true;
        AddLocalEvent(new LocationStatusChangedEvent
        {
            LocationId = Id,
            LocationCode = LocationCode,
            IsActive = true
        });
        return this;
    }

    /// <summary>
    /// Deactivates the location.
    /// </summary>
    public Location Deactivate()
    {
        IsActive = false;
        AddLocalEvent(new LocationStatusChangedEvent
        {
            LocationId = Id,
            LocationCode = LocationCode,
            IsActive = false
        });
        return this;
    }
}
