using Volo.Abp.Domain.Entities.Auditing;
using Wms.Warehouse.Domain.Enums;
using Wms.Warehouse.Domain.Events;

namespace Wms.Warehouse.Domain.Aggregates;

/// <summary>
/// Warehouse Area Aggregate Root (AGG-02, ENT-02) — represents a functional zone within a warehouse.
/// Inherits FullAuditedAggregateRoot for ABP audit fields and soft delete.
/// References Warehouse via ID + redundant Code (cross-aggregate reference pattern).
/// (Phase 3 DDD Design)
/// </summary>
public class WarehouseArea : FullAuditedAggregateRoot<Guid>
{
    /// <summary>库区编码（仓库内唯一）</summary>
    public string AreaCode { get; private set; } = string.Empty;

    /// <summary>库区名称</summary>
    public string AreaName { get; private set; } = string.Empty;

    /// <summary>所属仓库ID（跨聚合引用）</summary>
    public string WarehouseId { get; private set; } = string.Empty;

    /// <summary>所属仓库编码（冗余）</summary>
    public string WarehouseCode { get; private set; } = string.Empty;

    /// <summary>库区功能枚举值</summary>
    public int AreaFunction { get; private set; }

    /// <summary>存储环境枚举值</summary>
    public int StorageEnvironment { get; private set; }

    /// <summary>最大容量</summary>
    public decimal? MaxCapacity { get; private set; }

    /// <summary>当前容量</summary>
    public decimal? CurrentCapacity { get; private set; }

    /// <summary>是否启用</summary>
    public bool IsActive { get; private set; } = true;

    /// <summary>
    /// Creates a new WarehouseArea aggregate root.
    /// </summary>
    public WarehouseArea(
        Guid id,
        string areaCode,
        string areaName,
        string warehouseId,
        string warehouseCode,
        int areaFunction,
        int storageEnvironment = 0,
        decimal? maxCapacity = null,
        decimal? currentCapacity = null,
        bool isActive = true) : base(id)
    {
        SetAreaCode(areaCode);
        AreaName = areaName ?? throw new ArgumentNullException(nameof(areaName));
        WarehouseId = warehouseId ?? throw new ArgumentNullException(nameof(warehouseId));
        WarehouseCode = warehouseCode ?? throw new ArgumentNullException(nameof(warehouseCode));
        SetAreaFunction(areaFunction);
        StorageEnvironment = storageEnvironment;
        MaxCapacity = maxCapacity;
        CurrentCapacity = currentCapacity;
        IsActive = isActive;
    }

    /// <summary>
    /// Sets the area code. Must be non-empty.
    /// </summary>
    public WarehouseArea SetAreaCode(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("Area code cannot be empty.", nameof(code));
        AreaCode = code.Trim();
        return this;
    }

    /// <summary>
    /// Sets the area function from AreaFunction enum.
    /// </summary>
    public WarehouseArea SetAreaFunction(int areaFunction)
    {
        if (!Wms.Warehouse.Domain.Enums.AreaFunction.TryFromValue(areaFunction, out _))
            throw new ArgumentException($"Invalid area function value: {areaFunction}", nameof(areaFunction));
        AreaFunction = areaFunction;
        return this;
    }

    /// <summary>
    /// Sets the area name.
    /// </summary>
    public WarehouseArea SetAreaName(string areaName)
    {
        if (string.IsNullOrWhiteSpace(areaName))
            throw new ArgumentException("Area name cannot be empty.", nameof(areaName));
        AreaName = areaName.Trim();
        return this;
    }

    /// <summary>
    /// Sets the storage environment.
    /// </summary>
    public WarehouseArea SetStorageEnvironment(int storageEnvironment)
    {
        if (!Wms.Warehouse.Domain.Enums.StorageEnvironment.TryFromValue(storageEnvironment, out _))
            throw new ArgumentException($"Invalid storage environment value: {storageEnvironment}", nameof(storageEnvironment));
        StorageEnvironment = storageEnvironment;
        return this;
    }

    /// <summary>
    /// Updates the capacity values for this area.
    /// </summary>
    public WarehouseArea UpdateCapacity(decimal? maxCapacity, decimal? currentCapacity)
    {
        if (maxCapacity != null && currentCapacity != null && currentCapacity > maxCapacity)
            throw new ArgumentException("Current capacity cannot exceed max capacity.");
        MaxCapacity = maxCapacity;
        CurrentCapacity = currentCapacity;
        return this;
    }

    /// <summary>
    /// Activates the area.
    /// </summary>
    public WarehouseArea SetActive()
    {
        IsActive = true;
        return this;
    }

    /// <summary>
    /// Deactivates the area.
    /// </summary>
    public WarehouseArea Deactivate()
    {
        IsActive = false;
        return this;
    }
}
